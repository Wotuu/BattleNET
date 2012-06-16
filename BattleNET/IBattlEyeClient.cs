﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BattleNET
{
    public class IBattlEyeClient : IBattleNET
    {
        private Socket _socket;

        private DateTime _commandSend = DateTime.Now;
        private DateTime _responseReceived = DateTime.Now;

        private EBattlEyeDisconnectionType _disconnectionType;
        private bool _keepRunning;

        private void OnMessageReceived(string message)
        {
            if (MessageReceivedEvent != null)
                MessageReceivedEvent(new BattlEyeMessageEventArgs(message));
        }

        private void OnDisconnect(BattleEyeLoginCredentials loginDetails, EBattlEyeDisconnectionType disconnectionType)
        {
            if (DisconnectEvent != null)
                DisconnectEvent(new BattlEyeDisconnectEventArgs(loginDetails, disconnectionType));
        }

        private BattleEyeLoginCredentials _loginCredentials;

        public IBattlEyeClient(BattleEyeLoginCredentials loginCredentials)
        {
            _loginCredentials = loginCredentials;
        }

        private EBattlEyeCommandResult SendLoginPacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.Default.GetBytes(Helpers.Hex2Ascii("FF00") + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF00") + command;
                _socket.Send(Encoding.Default.GetBytes(packet));

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Succes;
        }

        private EBattlEyeCommandResult SendAcknowledgePacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.Default.GetBytes(Helpers.Hex2Ascii("FF02") + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF02") + command;
                _socket.Send(Encoding.Default.GetBytes(packet));

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Succes;
        }

        public EBattlEyeCommandResult SendCommandPacket(string command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash = crc32.ComputeHash(Encoding.Default.GetBytes(Helpers.Hex2Ascii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) + command)).Aggregate<byte, string>(null,
                                                                                                            (current, b)
                                                                                                            =>
                                                                                                            current +
                                                                                                            b.ToString(
                                                                                                                "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) + command;
                _socket.Send(Encoding.Default.GetBytes(packet));

                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Succes;
        }

        public EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash =
                    crc32.ComputeHash(
                        Encoding.Default.GetBytes(Helpers.Hex2Ascii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) +
                                                  Helpers.StringValueOf(command))).Aggregate<byte, string>(
                                                      null,
                                                      (current, b)
                                                      =>
                                                      current +
                                                      b.ToString(
                                                          "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) +
                         Helpers.StringValueOf(command);
                _socket.Send(Encoding.Default.GetBytes(packet));
                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Succes;
        }


        public EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command, string parameters)
        {
            try
            {
                if (!_socket.Connected)
                    return EBattlEyeCommandResult.NotConnected;

                var crc32 = new CRC32();
                string packet;
                string header = "BE";
                string hash =
                    crc32.ComputeHash(
                        Encoding.Default.GetBytes(Helpers.Hex2Ascii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) +
                                                  Helpers.StringValueOf(command) + parameters)).Aggregate
                        <byte, string>(null,
                                       (current, b)
                                       =>
                                       current +
                                       b.ToString(
                                           "X2"));
                hash = Helpers.Hex2Ascii(hash);
                hash = new string(hash.ToCharArray().Reverse().ToArray());
                header += hash;
                packet = header + Helpers.Hex2Ascii("FF01") + Encoding.Default.GetString(new byte[] { 0 }) +
                         Helpers.StringValueOf(command) + parameters;
                _socket.Send(Encoding.Default.GetBytes(packet));
                _commandSend = DateTime.Now;
            }
            catch
            {
                return EBattlEyeCommandResult.Error;
            }

            return EBattlEyeCommandResult.Succes;
        }

        public bool IsConnected()
        {
            return _socket.Connected;
        }

        public EBattlEyeConnectionResult Connect()
        {
            try
            {
                _keepRunning = true;
                IPAddress ipAddress = IPAddress.Parse(_loginCredentials.Host);
                EndPoint remoteEP = new IPEndPoint(ipAddress, _loginCredentials.Port);

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                OnMessageReceived(string.Format("Connecting to {0}:{1}... ", _loginCredentials.Host,
                                                _loginCredentials.Port));

                try
                {
                    _socket.Connect(remoteEP);

                    OnMessageReceived("Connected!");

                    OnMessageReceived("Logging in... ");

                    //SendCommand(Helpers.Hex2Ascii("FF00") + _loginCredentials.Password);
                    SendLoginPacket(_loginCredentials.Password);
                    new Thread(DoWork).Start();
                    new Thread(KeepAlive).Start();
                }
                catch (Exception)
                {
                    return EBattlEyeConnectionResult.ConnectionFailed;
                }
            }
            catch (Exception)
            {
                return EBattlEyeConnectionResult.ParseError;
            }

            return EBattlEyeConnectionResult.Succes;
        }

        public void Disconnect()
        {
            _keepRunning = false;
            _disconnectionType = EBattlEyeDisconnectionType.Manual;

            if (_socket.Connected)
                _socket.DisconnectAsync(new SocketAsyncEventArgs());

            OnDisconnect(_loginCredentials, _disconnectionType);
        }

        private void Disconnect(EBattlEyeDisconnectionType disconnectionType)
        {
            _keepRunning = false;
            _disconnectionType = disconnectionType;

            if (_socket.Connected)
                _socket.DisconnectAsync(new SocketAsyncEventArgs());

            //OnDisconnect(_loginCredentials, _disconnectionType);
        }

        private void DoWork()
        {
            var bytesReceived = new Byte[4096];
            int bytes = 0;

            string buffer = null;
            int bufferCount = 0;
            int packetCount = 0;
            _disconnectionType = EBattlEyeDisconnectionType.ConnectionLost;

            while (_socket.Connected && _keepRunning)
            {
                try
                {
                    bytes = _socket.Receive(bytesReceived, bytesReceived.Length, 0);

                    if (bytesReceived[7] == 0x00)
                    {
                        if (bytesReceived[8] == 0x01)
                        {
                            OnMessageReceived("Logged in!");
                        }
                        else
                        {
                            Disconnect(EBattlEyeDisconnectionType.LoginFailed);
                        }
                    }
                    else if (bytesReceived[7] == 0x02)
                    {
                        SendAcknowledgePacket(Encoding.Default.GetString(new[] { bytesReceived[8] }));
                        OnMessageReceived(Encoding.Default.GetString(bytesReceived, 9, bytes - 9));
                    }
                    else if (bytesReceived[7] == 0x01)
                    {
                        if (bytes > 9)
                        {
                            if (bytesReceived[7] == 0x01 && bytesReceived[9] == 0x00)
                            {
                                if (bytesReceived[11] == 0)
                                {
                                    packetCount = bytesReceived[10];
                                }

                                if (bufferCount < packetCount)
                                {
                                    buffer += Encoding.Default.GetString(bytesReceived, 12, bytes - 12);
                                    bufferCount++;
                                }

                                if (bufferCount == packetCount)
                                {
                                    OnMessageReceived(buffer);
                                    buffer = null;
                                    bufferCount = 0;
                                    packetCount = 0;
                                }
                            }
                            else
                            {
                                OnMessageReceived(Encoding.Default.GetString(bytesReceived, 9, bytes - 9));
                            }
                        }
                        else
                        {
                            // Response to keep alive packet which is currently not important to us
                        }
                    }

                    _responseReceived = DateTime.Now;
                    bytesReceived = new Byte[4096];
                }
                catch (Exception)
                {
                    Disconnect(EBattlEyeDisconnectionType.SocketException);
                }
            }

            if (!_socket.Connected)
                OnDisconnect(_loginCredentials, _disconnectionType);
        }

        private void KeepAlive()
        {
            while (_socket.Connected && _keepRunning)
            {
                TimeSpan timeoutClient = DateTime.Now - _commandSend;
                TimeSpan timeoutServer = DateTime.Now - _responseReceived;

                if (timeoutClient.TotalSeconds >= 30)
                {
                    SendCommandPacket(null);
                }

                if (timeoutServer.TotalSeconds >= 90)
                {
                    Disconnect(EBattlEyeDisconnectionType.ConnectionLost);
                    Console.WriteLine("Connection lost!");
                }

                Thread.Sleep(1000);
            }
        }

        public event BattlEyeMessageEventHandler MessageReceivedEvent;
        public event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}