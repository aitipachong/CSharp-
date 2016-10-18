using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aitipachong.Sys;
using System.Collections.Generic;

namespace UT_aitipachong.Sys
{
    [TestClass]
    public class UT_OSHelper
    {
        [TestMethod]
        public void UT_GetOsBit_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string osBit = helper.GetOsBit();
                Assert.AreEqual("64", osBit);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetSystemLoginUserName_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string user = helper.GetSystemLoginUserName();
                Assert.AreEqual(@"L0000014610\admin", user);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetTotalPhysicalMemory_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string memory = helper.GetTotalPhysicalMemory();
                Assert.AreEqual("5.88GB", memory);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetSystemSpace_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                List<string> divers = helper.GetSystemSpace(SystemSpace.TotalSpace);
                Assert.AreEqual(4, divers.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_ExecuteCmd_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string cmdResult = helper.ExecuteCmd("ipconfig", 0);
                int index = cmdResult.IndexOf("10.100.8.118");
                Assert.AreEqual(true, index > 0 ? true : false);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetSystemNetworkingState_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                bool isNetwork = helper.GetSystemNetworkingState();
                Assert.AreEqual(true, isNetwork);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetLocalInnerIP_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string innerIp = helper.GetLocalInnerIP();
                Assert.AreEqual("10.100.8.118\n", innerIp);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetLocalOuterIP_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string outerIp = helper.GetLocalOuterIP();
                Assert.AreEqual("1.85.38.99", outerIp);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetCpuNumber_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string cpu = helper.GetCpuNumber();
                Assert.AreEqual(true, cpu.Trim() == "unknown" ? false : true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetHardDiskID_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string harddisk = helper.GetHardDiskID();
                Assert.AreEqual(true, harddisk.Trim() == "unknown" ? false : true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetMac_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string mac = helper.GetMac();
                Assert.AreEqual(true, mac.Trim() == "unknown" ? false : true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetIPAddress_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string ipAddress = helper.GetIPAddress();
                Assert.AreEqual(true, ipAddress.Trim() == "unknown" ? false : true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void UT_GetSystemType_V1()
        {
            try
            {
                OSHelper helper = new OSHelper();
                string systemType = helper.GetSystemType();
                Assert.AreEqual(true, systemType.Trim() == "unknown" ? false : true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
