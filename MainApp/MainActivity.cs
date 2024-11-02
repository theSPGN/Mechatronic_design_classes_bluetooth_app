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
        BluetoothAdapter bluetoothAdapter;
        BluetoothSocket bluetoothSocket;
        BluetoothDevice bluetoothDevice;
        Stream outStream;
        ListView listViewDevices;
        ArrayAdapter<string> deviceListAdapter;
        List<BluetoothDevice> deviceList = new List<BluetoothDevice>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += ShowPairedDevices;

            listViewDevices = FindViewById<ListView>(Resource.Id.listViewDevices);
            listViewDevices.ItemClick += DeviceSelected;

            deviceListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            listViewDevices.Adapter = deviceListAdapter;

            Button buttonA = FindViewById<Button>(Resource.Id.buttonA);
            buttonA.Click += (sender, e) => SendData("a");

            Button buttonB = FindViewById<Button>(Resource.Id.buttonB);
            buttonB.Click += (sender, e) => SendData("b");

            Button buttonC = FindViewById<Button>(Resource.Id.buttonC);
            buttonC.Click += (sender, e) => SendData("c");

            Button buttonD = FindViewById<Button>(Resource.Id.buttonD);
            buttonD.Click += (sender, e) => SendData("d");
        }

        private void ShowPairedDevices(object sender, EventArgs e)
        {
            var pairedDevices = bluetoothAdapter.BondedDevices;
            deviceList.Clear();
            deviceListAdapter.Clear();

            if (pairedDevices.Count > 0)
            {
                foreach (var device in pairedDevices)
                {
                    deviceList.Add(device);
                    deviceListAdapter.Add(device.Name + "\n" + device.Address);
                }
            }

            listViewDevices.Visibility = Android.Views.ViewStates.Visible;
        }

        private void DeviceSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            bluetoothDevice = deviceList[e.Position];
            ConnectToBluetooth();
        }

        private void ConnectToBluetooth()
        {
            try
            {
                bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
                bluetoothSocket.Connect();
                outStream = bluetoothSocket.OutputStream;

                FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Visible;
                FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Visible;
                listViewDevices.Visibility = Android.Views.ViewStates.Gone;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Connection failed: " + ex.Message, ToastLength.Short).Show();
            }
        }

        private void SendData(string data)
        {
            if (outStream != null)
            {
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
                outStream.Write(buffer, 0, buffer.Length);
            }
        }
        private void DisconnectBluetooth()
        {
            if (bluetoothSocket != null)
            {
                bluetoothSocket.Close();
                bluetoothSocket = null;

                FindViewById<Button>(Resource.Id.buttonA).Visibility = Android.Views.ViewStates.Gone;
                FindViewById<Button>(Resource.Id.buttonB).Visibility = Android.Views.ViewStates.Gone;
                FindViewById<Button>(Resource.Id.buttonC).Visibility = Android.Views.ViewStates.Gone;
                FindViewById<Button>(Resource.Id.buttonD).Visibility = Android.Views.ViewStates.Gone;
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (bluetoothSocket != null)
            {
                DisconnectBluetooth();
            }
        }
    }
}
