using Xunit;
using SerialAssistant.Infrastructure.Configuration;
using SerialAssistant.Core.Models;
using System.IO;
using System.Text;

namespace SerialAssistant.Tests.Infrastructure
{
    /*
     * Tests for JsonAppSettingsService
     */
    public class JsonAppSettingsServiceTests
    {
        private string CreateTestFilePath()
        {
            return Path.Combine(Path.GetTempPath(), $"SerialAssistantTest_{Guid.NewGuid()}.json");
        }

        /*
         * Test 配置文件不存在时返回默认配置
         */
        [Fact]
        public void Load_FileNotExists_ReturnsDefault()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(9600, result.Value.BaudRate);
            Assert.Equal(8, result.Value.DataBits);
            Assert.Equal("None", result.Value.Parity);
            Assert.Equal("One", result.Value.StopBits);
        }

        /*
         * Test 保存后配置文件存在
         */
        [Fact]
        public void Save_AfterSave_FileExists()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);
            var settings = new AppSettings { BaudRate = 115200 };

            /* Act */
            var saveResult = service.Save(settings);

            /* Assert */
            Assert.True(saveResult.IsSuccess);
            Assert.True(File.Exists(testPath));

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 保存后再次加载能恢复配置
         */
        [Fact]
        public void Load_AfterSave_RestoresSettings()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);
            var originalSettings = new AppSettings
            {
                LastPortName = "COM3",
                BaudRate = 115200,
                DataBits = 7,
                Parity = "Odd",
                StopBits = "Two"
            };

            /* Act */
            service.Save(originalSettings);
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("COM3", result.Value.LastPortName);
            Assert.Equal(115200, result.Value.BaudRate);
            Assert.Equal(7, result.Value.DataBits);
            Assert.Equal("Odd", result.Value.Parity);
            Assert.Equal("Two", result.Value.StopBits);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 损坏的 JSON 文件不会导致未处理异常
         */
        [Fact]
        public void Load_CorruptJson_ReturnsDefault()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            File.WriteAllText(testPath, "{ invalid json }", Encoding.UTF8);
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(9600, result.Value.BaudRate);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test Save(null) 返回 Failure
         */
        [Fact]
        public void Save_NullSettings_ReturnsFailure()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Save(null!);

            /* Assert */
            Assert.False(result.IsSuccess);
            Assert.Contains("配置不能为空", result.ErrorMessage);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 默认配置包含预期值
         */
        [Fact]
        public void Load_DefaultConfig_ContainsExpectedValues()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(9600, result.Value.BaudRate);
            Assert.Equal(8, result.Value.DataBits);
            Assert.Equal("None", result.Value.Parity);
            Assert.Equal("One", result.Value.StopBits);
            Assert.Equal(Core.Enums.SendMode.Text, result.Value.SendMode);
            Assert.Equal(Core.Enums.DisplayMode.Text, result.Value.DisplayMode);
            Assert.Equal(Core.Enums.SendLineEnding.None, result.Value.SendLineEnding);
            Assert.True(result.Value.ShowTimestamp);
            Assert.True(result.Value.ShowDirection);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 保存后加载能恢复 SendLineEnding
         */
        [Fact]
        public void Load_AfterSave_RestoresSendLineEnding()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);
            var originalSettings = new AppSettings
            {
                SendLineEnding = Core.Enums.SendLineEnding.CRLF
            };

            /* Act */
            service.Save(originalSettings);
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(Core.Enums.SendLineEnding.CRLF, result.Value.SendLineEnding);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 旧配置文件缺少 SendLineEnding 字段时使用默认值 None
         */
        [Fact]
        public void Load_OldConfigWithoutSendLineEnding_UsesNone()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            string oldJson = @"{
                ""LastPortName"": ""COM3"",
                ""BaudRate"": 9600,
                ""DataBits"": 8,
                ""Parity"": ""None"",
                ""StopBits"": ""One"",
                ""SendMode"": 0,
                ""DisplayMode"": 0
            }";
            File.WriteAllText(testPath, oldJson, Encoding.UTF8);
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(Core.Enums.SendLineEnding.None, result.Value.SendLineEnding);
            Assert.True(result.Value.ShowTimestamp);
            Assert.True(result.Value.ShowDirection);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 保存后加载能恢复 ShowTimestamp
         */
        [Fact]
        public void Load_AfterSave_RestoresShowTimestamp()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);
            var originalSettings = new AppSettings
            {
                ShowTimestamp = false
            };

            /* Act */
            service.Save(originalSettings);
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.False(result.Value.ShowTimestamp);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 保存后加载能恢复 ShowDirection
         */
        [Fact]
        public void Load_AfterSave_RestoresShowDirection()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);
            var originalSettings = new AppSettings
            {
                ShowDirection = false
            };

            /* Act */
            service.Save(originalSettings);
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.False(result.Value.ShowDirection);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 旧配置文件缺少 ShowTimestamp 和 ShowDirection 时使用默认值 true
         */
        [Fact]
        public void Load_OldConfigWithoutShowSettings_UsesDefaultTrue()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            string oldJson = @"{
                ""LastPortName"": ""COM3"",
                ""BaudRate"": 9600,
                ""DataBits"": 8,
                ""Parity"": ""None"",
                ""StopBits"": ""One"",
                ""SendMode"": 0,
                ""DisplayMode"": 0,
                ""SendLineEnding"": 0
            }";
            File.WriteAllText(testPath, oldJson, Encoding.UTF8);
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.ShowTimestamp);
            Assert.True(result.Value.ShowDirection);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 默认配置 MaxDisplayBytes 为 262144
         */
        [Fact]
        public void Load_DefaultConfig_MaxDisplayBytes_Is262144()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(262144, result.Value.MaxDisplayBytes);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 保存后加载能恢复 MaxDisplayBytes
         */
        [Fact]
        public void Load_AfterSave_RestoresMaxDisplayBytes()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
            var service = new JsonAppSettingsService(testPath);
            var originalSettings = new AppSettings
            {
                MaxDisplayBytes = 1048576
            };

            /* Act */
            service.Save(originalSettings);
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(1048576, result.Value.MaxDisplayBytes);

            /* Cleanup */
            File.Delete(testPath);
        }

        /*
         * Test 旧配置文件缺少 MaxDisplayBytes 时使用默认值 262144
         */
        [Fact]
        public void Load_OldConfigWithoutMaxDisplayBytes_UsesDefault262144()
        {
            /* Arrange */
            string testPath = CreateTestFilePath();
            string oldJson = @"{
                ""LastPortName"": ""COM3"",
                ""BaudRate"": 9600,
                ""DataBits"": 8,
                ""Parity"": ""None"",
                ""StopBits"": ""One"",
                ""SendMode"": 0,
                ""DisplayMode"": 0,
                ""SendLineEnding"": 0
            }";
            File.WriteAllText(testPath, oldJson, Encoding.UTF8);
            var service = new JsonAppSettingsService(testPath);

            /* Act */
            var result = service.Load();

            /* Assert */
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(262144, result.Value.MaxDisplayBytes);

            /* Cleanup */
            File.Delete(testPath);
        }
    }
}
