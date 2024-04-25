## 简介
SBGT(Shining!BeautifulGirls-Tools)，是本人自用自制的手游《闪耀！优俊少女》（国服赛马娘）的辅助工具。
主要用途是自动化操作。减轻游玩负担，拒绝重复劳动。
- 源代码
- C#/.NET
- WPF

## 软件获取方式
#### 前往[SBGT-Release](https://github.com/Miltomo/SBGT-Release)直接下载发布版本或自行构建

---
## 贡献
### ★编写新的养成算法★
1. 在`src/girl/Logic`文件夹下新建`你的算法.cs`或直接复制`Template.cs`
2. 你需要在`ShiningGirl`类下新建一个属于你的算法类，并继承自`ShiningGirl.Algorithm`。具体代码如下：
    ```C#
    namespace Shining_BeautifulGirls
    {
        partial class ShiningGirl
        {
            class 你的算法 : Algorithm
            {
            }
        }
    }
    ```
3. 编写算法主体：你最少需要实现`Score`和`Select`这两个函数：  
`Score`接受一个`Plan`变量，返回`double`，内容是为一个计划打分；   
`Select`为无参数函数，返回`Plan`，内容是选择一项合适的计划。   
整个算法的实现过程就是先为所有可能的项目打分，最后根据得分(或其他因素)，选出最合适的要执行的项目。  
具体代码如下：
    ```C#
    class YourAlgorithm : Algorithm
    {
        // 构造函数
        public YourAlgorithm(ShiningGirl girl) : base(girl)
        {
        }
    
        // 实现你的打分逻辑
        protected override double Score(Plan info)
        {
            throw new NotImplementedException();
        }
    
        // 实现你的选择逻辑
        protected override Plan Select()
        {
            throw new NotImplementedException();
        }
    }
    ```
4. 注册算法：找到[`Shining BeautifulGirls\src\girl\Logic\Base\AlgorithmItem.cs`](https://github.com/Miltomo/Shining-BeautifulGirls-Tools/blob/master/src/girl/Logic/Base/AlgorithmItem.cs)，在其中的`代号Enum`里添加新的代号  
   然后找到`public static AlgorithmItem[] Algorithms`，在其中添加新项，`AlgorithmItem.Build("算法名","算法介绍",AlgorithmItem.代号Enum.算法代号)`
5. API参考：详见[`..\girl\Logic\Base\Base.cs`](https://github.com/Miltomo/Shining-BeautifulGirls-Tools/blob/master/src/girl/Logic/Base/Base.cs)。  
注意，作为规则，只能使用基类`Algorithm`提供的变量与函数，不可直接用`girl`调用，更一般的，除构造函数外，不应再访问`girl`变量。