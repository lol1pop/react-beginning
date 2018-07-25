using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DeskAlerts
{
    public class IPAddressRange
    {
        private readonly AddressFamily addressFamily;
        private readonly byte[] lowerBytes;
        private readonly byte[] upperBytes;

        public IPAddressRange(IPAddress lowerInclusive, IPAddress upperInclusive) {
            if (lowerInclusive.AddressFamily == upperInclusive.AddressFamily) {
                addressFamily = lowerInclusive.AddressFamily;
                lowerBytes = lowerInclusive.GetAddressBytes();
                upperBytes = upperInclusive.GetAddressBytes();
            }
            else {
                addressFamily = AddressFamily.Unknown;
            }
        }

        public bool IsInRange(IPAddress address) {
            if (address.AddressFamily != addressFamily) return false;

            var addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (var i = 0;
                i < lowerBytes.Length &&
                (lowerBoundary || upperBoundary);
                i++) {
                if (lowerBoundary && addressBytes[i] < lowerBytes[i] ||
                    upperBoundary && addressBytes[i] > upperBytes[i])
                    return false;

                lowerBoundary &= addressBytes[i] == lowerBytes[i];
                upperBoundary &= addressBytes[i] == upperBytes[i];
            }

            return true;
        }
    }

    public class DeviceManager
    {
        private readonly UserManager UserManager;

        public DeviceManager(UserManager UserManager) {
            DeviceList = new List<UserDevice>();
            this.UserManager = UserManager;
        }

        public List<UserDevice> DeviceList { get; }

        /// <exception cref="Exception">
        ///     <paramref name="Device" /> is <see langword="null" />.
        /// </exception>
        public void OnDeviceDisconnect(UserDevice Device) {
            if (Device == null)
                throw new Exception("Nu ti i pidor"); //TODO: DeskAlerts exaptions
            if (Device.IsAuth)
                UserManager.UserDeviceDisconnected(Device);
            DeviceList.Remove(Device);
        }

        /// <exception cref="Exception">
        ///     <paramref name="IPRange" /> is <see langword="null" />.
        /// </exception>
        public List<UserDevice> GetDeviceByRange(IPAddressRange IPRange) {
            if (IPRange == null)
                throw new Exception("Nu ti i pidor"); //TODO: DeskAlerts exaptions
            var resultData = new List<UserDevice>();
            foreach (var device in DeviceList)
                if (IPRange.IsInRange(device.Context.UserEndPoint.Address))
                    resultData.Add(device);
            return resultData;
        }

        public UserDevice OnDeviceConnect() {
            var Device = new UserDevice();
            Device.onClientConnect += UserManager.UserDeviceConnected;
            Device.onClientDisconnected += OnDeviceDisconnect;
            DeviceList.Add(Device);
            return Device;
        }
    }
}