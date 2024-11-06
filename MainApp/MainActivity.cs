using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content.PM;
using AndroidX.Annotations;
using Android.Views;

namespace MainApp
{
    [Activity(Label = "BluetoothApp", MainLauncher = true)]
    public class MainActivity : Activity
    {
        BluetoothAdapter bluetoothAdapter; // Adapter Bluetooth
        BluetoothSocket bluetoothSocket; // Gniazdo Bluetooth
        BluetoothDevice bluetoothDevice; // Urządzenie Bluetooth
        Stream outStream; // Strumień wyjściowy do wysyłania danych
        ListView listViewDevices; // Lista urządzeń Bluetooth
        ArrayAdapter<string> deviceListAdapter; // Adapter dla listy urządzeń
        List<BluetoothDevice> deviceList = new List<BluetoothDevice>(); // Lista urządzeń Bluetooth
        BluetoothStateReceiver bluetoothStateReceiver; // Odbiornik stanu Bluetooth
        BluetoothDiscoveryReceiver discoveryReceiver; // Odbiornik niepodłączonych urządzeń
        const int RequestLocationId = 0;
        int _layout = Resource.Layout.manual;
        String Connection_action = "Connect";

        string[] PermissionsLocation =
        {
            Android.Manifest.Permission.AccessFineLocation,
            Android.Manifest.Permission.AccessCoarseLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(_layout); // Ustawienie widoku aktywności

            GetLocationPermission();


            // Inicjalizacja odbiornika odkrywania urządzeń
            discoveryReceiver = new BluetoothDiscoveryReceiver();
            discoveryReceiver.DeviceFound += OnDeviceFound;

            RegisterReceiver(discoveryReceiver, new IntentFilter(BluetoothDevice.ActionFound));


            bluetoothAdapter = BluetoothAdapter.DefaultAdapter; // Pobranie domyślnego adaptera Bluetooth

            bluetoothStateReceiver = new BluetoothStateReceiver(); // Inicjalizacja odbiornika stanu Bluetooth
            bluetoothStateReceiver.BluetoothDisabled += DisconnectBluetooth; // Subskrypcja zdarzenia wyłączenia Bluetooth

            RegisterReceiver(bluetoothStateReceiver, new IntentFilter(BluetoothAdapter.ActionStateChanged)); // Rejestracja odbiornika stanu Bluetooth

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect); // Pobranie przycisku połączenia
            buttonConnect.Click += (sender, e) => CheckBluetoothState(); // Ustawienie obsługi kliknięcia przycisku połączenia

            listViewDevices = FindViewById<ListView>(Resource.Id.listViewDevices); // Pobranie listy urządzeń
            listViewDevices.ItemClick += DeviceSelected; // Ustawienie obsługi kliknięcia elementu listy

            deviceListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1); // Inicjalizacja adaptera listy urządzeń
            listViewDevices.Adapter = deviceListAdapter; // Ustawienie adaptera dla listy urządzeń

            // Pobranie przycisków i ustawienie obsługi kliknięć
            Button buttonA = FindViewById<Button>(Resource.Id.buttonA);
            buttonA.Touch += (sender, e) => SendData("a");

            Button buttonB = FindViewById<Button>(Resource.Id.buttonB);
            buttonB.Touch += (sender, e) => SendData("b");

            Button buttonC = FindViewById<Button>(Resource.Id.buttonC);
            buttonC.Touch += (sender, e) => SendData("c");

            Button buttonD = FindViewById<Button>(Resource.Id.buttonD);
            buttonD.Touch += (sender, e) => SendData("d");

            Button buttonX = FindViewById<Button>(Resource.Id.buttonX);
            buttonX.Click += (sender, e) => SendData("x");
            
            Button buttonAutomatic = FindViewById<Button>(Resource.Id.buttonAutomatic);
            buttonAutomatic.Click += (sender, e) => SwitchWindow("automatic");

            // Ukrycie przycisków na początku
            FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonX).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonAutomatic).Visibility = Android.Views.ViewStates.Gone;
        }
        void GetLocationPermission()
        {
            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != (int)Permission.Granted ||
                CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            // Uprawnienia zostały przyznane
                            DiscoverDevices();
                        }
                        else
                        {
                            // Uprawnienia nie zostały przyznane
                            Toast.MakeText(this, "Location permissions are required to discover Bluetooth devices.", ToastLength.Short).Show();
                        }
                    }
                    break;
            }
        }

        private void DiscoverDevices()
        {
            if (bluetoothAdapter.IsDiscovering)
            {
                bluetoothAdapter.CancelDiscovery();
            }
            bool started = bluetoothAdapter.StartDiscovery();
        }

        private void OnDeviceFound(BluetoothDevice device)
        {
            if (!deviceList.Any(d => d.Address == device.Address))
            {
                deviceList.Add(device);
                deviceListAdapter.Add(device.Name + "\n" + device.Address);
            }
        }

        private void DeviceSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            bluetoothDevice = deviceList[e.Position]; // Pobranie wybranego urządzenia
            ConnectToBluetooth(); // Połączenie z wybranym urządzeniem
        }

        private void ConnectToBluetooth()
        {
            try
            {
                DisconnectBluetooth(); // Rozłączenie z interfejsem w przypadku próby zmiany urządzenia

                // Utworzenie nowego gniazda Bluetooth i połączenie
                bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
                bluetoothSocket.Connect();
                outStream = bluetoothSocket.OutputStream; // Pobranie strumienia wyjściowego

                // Zmiana napisu na przycisku
                Connection_action = "Disconnect";
                FindViewById<Button>(Resource.Id.buttonConnect).Text = Connection_action;

                // Uwidocznienie przycisków po połączeniu
                if (_layout == Resource.Layout.manual)
                {
                    FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonX).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonAutomatic).Visibility = Android.Views.ViewStates.Visible;
                }
                else if (_layout == Resource.Layout.automatic)
                {
                    FindViewById<Button>(Resource.Id.buttonQ).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonW).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonE).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonR).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonT).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonY).Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<Button>(Resource.Id.buttonManual).Visibility = Android.Views.ViewStates.Visible;
                }
                listViewDevices.Visibility = Android.Views.ViewStates.Gone; // Ukrycie listy urządzeń

                DiscoverDevices(); // Rozpoczęcie odkrywania urządzeń
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Connection failed: " + ex.Message, ToastLength.Short).Show(); // Wyświetlenie komunikatu o błędzie
                DisconnectBluetooth(); // Rozłączenie interfejsu w przypadku błędów komunikacyjnych
            }
        }

        private void SendData(string data)
        {
            if (outStream != null && bluetoothSocket != null && bluetoothSocket.IsConnected)
            {
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data); // Konwersja danych na bajty
                outStream.Write(buffer, 0, buffer.Length); // Wysłanie danych
            }
        }

        private void SwitchWindow(string data)
        {
            _layout = (data == "manual") ? Resource.Layout.manual : Resource.Layout.automatic;
            SetContentView(_layout);

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect); // Pobranie przycisku połączenia
            buttonConnect.Click += (sender, e) => CheckBluetoothState(); // Ustawienie obsługi kliknięcia przycisku połączenia
            FindViewById<Button>(Resource.Id.buttonConnect).Text = Connection_action;

            listViewDevices = FindViewById<ListView>(Resource.Id.listViewDevices); // Pobranie listy urządzeń
            listViewDevices.ItemClick += DeviceSelected; // Ustawienie obsługi kliknięcia elementu listy

            deviceListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1); // Inicjalizacja adaptera listy urządzeń
            listViewDevices.Adapter = deviceListAdapter; // Ustawienie adaptera dla listy urządzeń


            if (_layout == Resource.Layout.automatic)
            {
                Button buttonQ = FindViewById<Button>(Resource.Id.buttonQ);
                buttonQ.Click += (sender, e) => SendData("q");

                Button buttonW = FindViewById<Button>(Resource.Id.buttonW);
                buttonW.Click += (sender, e) => SendData("w");

                Button buttonE = FindViewById<Button>(Resource.Id.buttonE);
                buttonE.Click += (sender, e) => SendData("e");

                Button buttonR = FindViewById<Button>(Resource.Id.buttonR);
                buttonR.Click += (sender, e) => SendData("r");

                Button buttonT = FindViewById<Button>(Resource.Id.buttonT);
                buttonT.Click += (sender, e) => SendData("t");

                Button buttonY = FindViewById<Button>(Resource.Id.buttonY);
                buttonY.Click += (sender, e) => SendData("y");

                Button buttonManual = FindViewById<Button>(Resource.Id.buttonManual);
                buttonManual.Click += (sender, e) => SwitchWindow("manual");
            }
            else if (_layout == Resource.Layout.manual)
            {
                Button buttonA = FindViewById<Button>(Resource.Id.buttonA);
                buttonA.Touch += (sender, e) => SendData("a");

                Button buttonB = FindViewById<Button>(Resource.Id.buttonB);
                buttonB.Touch += (sender, e) => SendData("b");

                Button buttonC = FindViewById<Button>(Resource.Id.buttonC);
                buttonC.Touch += (sender, e) => SendData("c");

                Button buttonD = FindViewById<Button>(Resource.Id.buttonD);
                buttonD.Touch += (sender, e) => SendData("d");

                Button buttonX = FindViewById<Button>(Resource.Id.buttonX);
                buttonX.Click += (sender, e) => SendData("x");

                Button buttonAutomatic = FindViewById<Button>(Resource.Id.buttonAutomatic);
                buttonAutomatic.Click += (sender, e) => SwitchWindow("automatic");
            }

        }

        private void ShowPairedDevices()
        {
            var pairedDevices = bluetoothAdapter.BondedDevices; // Pobranie sparowanych urządzeń
            deviceList.Clear();
            deviceListAdapter.Clear();
            GetLocationPermission();
            DiscoverDevices();

            if (pairedDevices.Count > 0)
            {
                foreach (var device in pairedDevices)
                {
                    deviceList.Add(device); // Dodanie urządzenia do listy
                    deviceListAdapter.Add(device.Name + "\n" + device.Address); // Dodanie urządzenia do adaptera listy
                }
            }

            listViewDevices.Visibility = Android.Views.ViewStates.Visible; // Uwidocznienie listy urządzeń
        }

        private void CheckBluetoothState()
        {
            // Wyłączenie interfejsu i zmiana napisu "Dissconnect" na "Connect"
            DisconnectBluetooth();
            Connection_action = "Connect";
            FindViewById<Button>(Resource.Id.buttonConnect).Text = Connection_action;

            if (!bluetoothAdapter.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable); // Utworzenie intencji włączenia Bluetooth
                StartActivityForResult(enableBtIntent, 1); // Rozpoczęcie aktywności włączania Bluetooth
            }
            else
            {
                ShowPairedDevices(); // Wyświetlenie sparowanych urządzeń
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    ShowPairedDevices(); // Wyświetlenie sparowanych urządzeń po włączeniu Bluetooth
                }
                else
                {
                    Toast.MakeText(this, "Bluetooth must be enabled to connect.", ToastLength.Short).Show(); // Wyświetlenie komunikatu o konieczności włączenia Bluetooth
                }
            }
        }

        private void DisconnectBluetooth()
        {
            if (bluetoothSocket != null)
            {
                bluetoothSocket.Close(); // Zamknięcie połączenia Bluetooth
                bluetoothSocket = null;

                // Ukrycie przycisków po rozłączeniu
                if (_layout == Resource.Layout.manual)
                {
                    FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonX).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonAutomatic).Visibility = Android.Views.ViewStates.Gone;
                }
                else if (_layout == Resource.Layout.automatic)
                {
                    FindViewById<Button>(Resource.Id.buttonQ).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonW).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonE).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonR).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonT).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonY).Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<Button>(Resource.Id.buttonManual).Visibility = Android.Views.ViewStates.Gone;
                }
                listViewDevices.Visibility = Android.Views.ViewStates.Gone; // Ukrycie listy urządzeń
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DisconnectBluetooth(); // Rozłączenie Bluetooth przy niszczeniu aktywności
            UnregisterReceiver(bluetoothStateReceiver); // Wyrejestrowanie odbiornika stanu Bluetooth
            UnregisterReceiver(discoveryReceiver); // Wyrejestrowanie odbiornika odkrywania urządzeń
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { BluetoothDevice.ActionFound })]
    public class BluetoothDiscoveryReceiver : BroadcastReceiver
    {
        public event Action<BluetoothDevice> DeviceFound;
        public event Action DiscoveryFinished;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == BluetoothDevice.ActionFound)
            {
                var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                DeviceFound?.Invoke(device);
            }
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { BluetoothAdapter.ActionStateChanged })]
    public class BluetoothStateReceiver : BroadcastReceiver
    {
        public event Action BluetoothDisabled; // Zdarzenie wyłączenia Bluetooth

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == BluetoothAdapter.ActionStateChanged)
            {
                var state = intent.GetIntExtra(BluetoothAdapter.ExtraState, BluetoothAdapter.Error); // Pobranie stanu Bluetooth
                if (state == (int)State.Off)
                {
                    BluetoothDisabled?.Invoke(); // Wywołanie zdarzenia wyłączenia Bluetooth
                }
            }
        }
    }
}