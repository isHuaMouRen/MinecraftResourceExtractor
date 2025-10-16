# MinecraftResourceExtractor

![Head](/MinecraftResourceExtractor/Assets/readme/headImage.png)

**Minecraft散列资源文件提取器**，用于提取Minecraft没有打包进`.jar`文件内的[散列资源文件](https://zh.minecraft.wiki/w/散列资源文件)，尤其是**音效、音乐**等

## ✨特色功能
- ⚡ 软件总大小**不足5MB**无需安装直接使用
- 🧩 自动识别`.minecraft`文件夹结构，无需翻找文件夹
- 🖼 全图形化界面，小白也可快速上手
- 💾 一键或批量导出资源
- 🎞 支持预览**图片、文本、音频**


## 📦 下载与运行
前往[Release界面](https://github.com/isHuaMouRen/MinecraftResourceExtractor/releases)获取最新版本的zip压缩包。解压到任意文件夹运行`MinecraftResourceExtractor.exe`

*(仅支持基于x86或x64的**现代Windows操作系统**)*

## 😀使用前后对比

### 🤔没有使用此工具

假如你想获得游戏的音效，最好的方法就是提取文件😋。那么你需要进行如下操作:

- 进入`.minecraft\assets\indexes`，打开目标版本的索引文件
- 寻找目标音效的哈希值
- 进入`.minecraft\assets\objects`里寻找目标文件
- 将复制到其他地方，改名 *(如果你能忍受那一大串文件名，那么此步骤可以省略)*
- 完成提取

这听着就非常麻烦😩，尤其是在批量导出的情况下。这样操作会使你的手和电脑报废🤯


### 😎使用了此工具
![Tip1](/MinecraftResourceExtractor/Assets/readme/tip1.png)

- 打开程序，直接点击右上角浏览按钮选择`.minecraft`文件夹


![Tip2](/MinecraftResourceExtractor/Assets/readme/tip2.png)

- 选择完毕后会让你选择索引文件，这里每一个索引文件对应一个或多个游戏版本。如果不知道怎么选择，可以点击上方按钮，或点击[此处](https://zh.minecraft.wiki/w/散列资源文件#索引名称)查看帮助


![Tip3](/MinecraftResourceExtractor/Assets/readme/tip3.png)

- 稍等几秒，左侧的文件列表就会加载出来，你可以查看所有散列资源文件


![Tip4](/MinecraftResourceExtractor/Assets/readme/tip4.png)

- 随意选择一个项，右侧就会显示文件预览，可支持 **图片、文本、音频** 的预览


![Tip5](/MinecraftResourceExtractor/Assets/readme/tip5.png)

- 点击下方的导出按钮并选择到处位置。你就可以成功地导出此文件了。不用再去objects文件夹去对哈希了🥰


![Tip6](/MinecraftResourceExtractor/Assets/readme/tip6.png)
![Tip7](/MinecraftResourceExtractor/Assets/readme/tip7.png)

- 一个个导出还是太慢了，你可以点击左上角的 `导出所有` 按钮。来导出所有可导出的文件(因为有时文件缺失，有几十个导出失败是正常的)


按最简操作路径，也就是导出游戏图标，那么你只用**点击鼠标5次**就可以成功导出文件🤓！


## ⚙依赖项
*(除`.NET8 Runtime`之外，其他的依赖项应已在发布时包含)*

- [.NET8 Runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)
- ModernWPFUI: [Github](https://github.com/Kinnara/ModernWpf) [Nuget](https://www.nuget.org/packages/ModernWpfUI)
- HuaZisToolLib.NET: [Github](https://github.com/isHuaMouRen/GeneralFunctions) [Nuget](https://www.nuget.org/packages/HuaZisToolLib.NET)
- [NAudio](https://www.nuget.org/packages/NAudio)
- [Newtonsoft.Json](https://www.nuget.org/packages/newtonsoft.json)

## 📜许可证
本项目基于[MIT开源协议](/LICENSE)