using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.IO;



namespace MainApp
{
    [Activity(Label = "BluetoothApp", MainLauncher = true)]
    public class MainActivity : Activity
    {
        BluetoothAdapter bluetoothAdapter;
        BluetoothSocket bluetoothSocket;
        BluetoothDevice bluetoothDevice;
        Stream outStream;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += ConnectToBluetooth;

            Button buttonA = FindViewById<Button>(Resource.Id.buttonA);
            buttonA.Click += (sender, e) => SendData("a");

            Button buttonB = FindViewById<Button>(Resource.Id.buttonB);
            buttonB.Click += (sender, e) => SendData("b");

            Button buttonC = FindViewById<Button>(Resource.Id.buttonC);
            buttonC.Click += (sender, e) => SendData("c");

            Button buttonD = FindViewById<Button>(Resource.Id.buttonD);
            buttonD.Click += (sender, e) => SendData("d");
        }

        private void ConnectToBluetooth(object sender, EventArgs e)
        {
            bluetoothDevice = bluetoothAdapter.GetRemoteDevice("98:DA:60:00:CA:40"); // adres MAC modułu HC-05
            bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
            bluetoothSocket.Connect();
            outStream = bluetoothSocket.OutputStream;
        }

        private void SendData(string data)
        {
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
            outStream.Write(buffer, 0, buffer.Length);
        }
    }
}
