using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main); // Ustawienie widoku aktywności

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
            buttonA.Click += (sender, e) => SendData("a");

            Button buttonB = FindViewById<Button>(Resource.Id.buttonB);
            buttonB.Click += (sender, e) => SendData("b");

            Button buttonC = FindViewById<Button>(Resource.Id.buttonC);
            buttonC.Click += (sender, e) => SendData("c");

            Button buttonD = FindViewById<Button>(Resource.Id.buttonD);
            buttonD.Click += (sender, e) => SendData("d");

            // Ukrycie przycisków na początku
            FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Gone;
            FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Gone;
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
                if (bluetoothSocket != null && bluetoothSocket.IsConnected)
                {
                    bluetoothSocket.Close(); // Zamknięcie istniejącego połączenia
                    bluetoothSocket = null;
                }

                // Utworzenie nowego gniazda Bluetooth i połączenie
                bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
                bluetoothSocket.Connect();
                outStream = bluetoothSocket.OutputStream; // Pobranie strumienia wyjściowego

                // Uwidocznienie przycisków po połączeniu
                FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Visible;
                listViewDevices.Visibility = Android.Views.ViewStates.Gone; // Ukrycie listy urządzeń
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Connection failed: " + ex.Message, ToastLength.Short).Show(); // Wyświetlenie komunikatu o błędzie
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

        private void ShowPairedDevices()
        {
            var pairedDevices = bluetoothAdapter.BondedDevices; // Pobranie sparowanych urządzeń
            deviceList.Clear();
            deviceListAdapter.Clear();

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
                FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Gone;
                FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Gone;
                FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Gone;
                FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Gone;
                listViewDevices.Visibility = Android.Views.ViewStates.Gone; // Ukrycie listy urządzeń
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DisconnectBluetooth(); // Rozłączenie Bluetooth przy niszczeniu aktywności
            UnregisterReceiver(bluetoothStateReceiver); // Wyrejestrowanie odbiornika stanu Bluetooth
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
