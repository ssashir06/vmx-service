using VMXService.Service;
using VMXService.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VIX_API_Test
{
    /// <summary>
    ///IVMControllerTest のテスト クラスです。すべての
    ///IVMControllerTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class IVMControllerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        // 
        //テストを作成するときに、次の追加属性を使用することができます:
        //
        //クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //各テストを実行する前にコードを実行するには、TestInitialize を使用
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //各テストを実行した後にコードを実行するには、TestCleanup を使用
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        internal virtual IVMController CreateIVMController()
        {
            // TODO: 適切な具象クラスをインスタンス化します。
            IVMController target = new VMControllerByAPI(new VMWareInfo().VMCore);
            return target;
        }

        /// <summary>
        ///IsRunning のテスト
        ///</summary>
        [TestMethod()]
        public void IsRunningTest()
        {
            IVMController target = CreateIVMController();
            IVMController vmrun = new VMControllerByVMRun(new VMWareInfo().VMCore);

            string vmx = @"C:\UserData\data\disk-image\Virtual Machines\Ubuntu8.04LTS 64\Ubuntu8.04LTS 64.vmx";
            bool expected = vmrun.IsRunning(vmx);
            bool actual;
            actual = target.IsRunning(vmx);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        }
        
        /// <summary>
        ///StopVMX のテスト
        ///</summary>
        [TestMethod()]
        public void StopVMXTest()
        {
            IVMController target = CreateIVMController();
            string vmx = @"C:\UserData\data\disk-image\Virtual Machines\Ubuntu8.04LTS 64\Ubuntu8.04LTS 64.vmx";
            bool expected = true;
            bool actual;
            actual = target.StopVMX(vmx);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        }


        /// <summary>
        ///StartVMX のテスト
        ///</summary>
        [TestMethod()]
        public void StartVMXTest()
        {
            IVMController target = CreateIVMController();
            string vmx = @"C:\UserData\data\disk-image\Virtual Machines\Ubuntu8.04LTS 64\Ubuntu8.04LTS 64.vmx";
            bool expected = true; // TODO: 適切な値に初期化してください
            bool actual;
            actual = target.StartVMX(vmx);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        }

        /// <summary>
        ///PauseVMX のテスト
        ///</summary>
        [TestMethod()]
        public void PauseVMXTest()
        {
            IVMController target = CreateIVMController();
            string vmx = @"C:\UserData\data\disk-image\Virtual Machines\Ubuntu8.04LTS 64\Ubuntu8.04LTS 64.vmx";
            bool expected = true;
            bool actual;
            actual = target.PauseVMX(vmx);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        }

        /// <summary>
        ///ContinueVMX のテスト
        ///</summary>
        [TestMethod()]
        public void ContinueVMXTest()
        {
            IVMController target = CreateIVMController();
            string vmx = @"C:\UserData\data\disk-image\Virtual Machines\Ubuntu8.04LTS 64\Ubuntu8.04LTS 64.vmx";
            bool expected = true;
            bool actual;
            actual = target.ContinueVMX(vmx);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        }
    }
}
