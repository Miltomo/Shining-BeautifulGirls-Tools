## ���
SBGT(Shining!BeautifulGirls-Tools)���Ǳ����������Ƶ����Ρ���ҫ���ſ���Ů��������������ĸ������ߡ�
��Ҫ��;���Զ����������������渺�����ܾ��ظ��Ͷ���
- Դ����
- C#/.NET
- WPF

## �����ȡ��ʽ
#### ǰ��[SBGT-Release](https://github.com/Miltomo/SBGT-Release)ֱ�����ط����汾�����й���

---
## ����
### ���д�µ������㷨��
1. ��`src/girl/Logic`�ļ������½�`����㷨.cs`��ֱ�Ӹ���`Template.cs`
2. ����Ҫ��`ShiningGirl`�����½�һ����������㷨�࣬���̳���`ShiningGirl.Algorithm`������������£�
    ```C#
    namespace Shining_BeautifulGirls
    {
        partial class ShiningGirl
        {
            class ����㷨 : Algorithm
            {
            }
        }
    }
    ```
3. ��д�㷨���壺��������Ҫʵ��`Score`��`Select`������������  
`Score`����һ��`Plan`����������`double`��������Ϊһ���ƻ���֣�   
`Select`Ϊ�޲�������������`Plan`��������ѡ��һ����ʵļƻ���   
�����㷨��ʵ�ֹ��̾�����Ϊ���п��ܵ���Ŀ��֣������ݵ÷�(����������)��ѡ������ʵ�Ҫִ�е���Ŀ��  
����������£�
    ```C#
    class YourAlgorithm : Algorithm
    {
        // ���캯��
        public YourAlgorithm(ShiningGirl girl) : base(girl)
        {
        }
    
        // ʵ����Ĵ���߼�
        protected override double Score(Plan info)
        {
            throw new NotImplementedException();
        }
    
        // ʵ�����ѡ���߼�
        protected override Plan Select()
        {
            throw new NotImplementedException();
        }
    }
    ```
4. ע���㷨���ҵ�[`Shining BeautifulGirls\src\girl\Logic\Base\AlgorithmItem.cs`](https://github.com/Miltomo/Shining-BeautifulGirls-Tools/blob/master/src/girl/Logic/Base/AlgorithmItem.cs)�������е�`����Enum`������µĴ���  
   Ȼ���ҵ�`public static AlgorithmItem[] Algorithms`��������������`AlgorithmItem.Build("�㷨��","�㷨����",AlgorithmItem.����Enum.�㷨����)`
5. API�ο������[`..\girl\Logic\Base\Base.cs`](https://github.com/Miltomo/Shining-BeautifulGirls-Tools/blob/master/src/girl/Logic/Base/Base.cs)��  
ע�⣬��Ϊ����ֻ��ʹ�û���`Algorithm`�ṩ�ı����뺯��������ֱ����`girl`���ã���һ��ģ������캯���⣬��Ӧ�ٷ���`girl`������