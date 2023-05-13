# Chapter 8 需求 Requirements

![](./\Chapter 8 需求 Requirements\Chapter_08-new.jpg)

- Modeling中的Analysis Modeling(分析建模)（功能建模，数据建模，行为建模）

需求阶段的2个work product——2份文档：

- Requirement Specification 需求规约
- Requirement Analysis Specification 需求分析规约

## 8.1 需求工程Requirement Engineering

包括：

- requirement elicitation需求诱导
- requirement analysis modeling需求分析建模

定义：一系列导出并理解需求的广泛的技术或任务(task) 是需求工程

涉及到framework activity的部分: 

- Communication
- Planning
- modeling

tasks set(需求工程的7个任务——task set原来的6个再加一个管理）:

1. Inception 初始化——项目启动阶段
2. elicitation 诱导——需求调研
3. elaboration 详细阐述——细化进行建模
4. negotiation 谈判——需求谈判
5. specification 规约——形成2个规约
    1. 需求规约——文字描述，UseCase
    2. 需求分析规约——文字描述，功能建模，数据建模，行为建模
6. validation 验证——对规约进行评审（Formal Technical Review正规技术评审(见指南v1.0)）
7. management 管理——对需求进行版本控制和管理

(7个任务是并行的parallel)

### 8.1.1 Inception 项目启动阶段

目的：

1. Target 确定项目总的目标
2. Communication  和stakeholder甲方进行初步有效的沟通

做的事情：

1. Identifying Stakeholders 明确甲方
2. Recognizing multiple viewpoints 辨认多个视点
3. Working toward collaboration 协作
4. Asking the first questions 
5. Nonfunctional requirements 考虑非功能需求

明确跟你提要求的人，然后明确问题，从多个角度看问题。之后寻求和甲方的协作。协作达成后，考虑一些初始的问题，最后考虑非功能需求（这一大部分 可能不重要）

### 8.1.2 Elicitation 需求调研(诱导)

概况：针对上一个阶段确定的目标来进行需求调研

组成: 

1. 确定业务目标establish business goals（要驱动甲方尽量不保留地表达他们的目标）
2. 建立：
    1. 优先级机制:Prioritization mechanism
       
        <img src="./\Chapter 8 需求 Requirements\Untitled.png" style="zoom: 33%;" />
        
    2. 一套潜在的软件系统架构设计依据design rationale for a potential arch
       
        <img src="./\Chapter 8 需求 Requirements\Untitled 1.png" style="zoom:33%;" />
        
        <img src="./\Chapter 8 需求 Requirements\Untitled 2.png" alt="Untitled 2" style="zoom:33%;" />
    
    （上图：一套潜在的软件体系架构）
    

可能碰到的问题：

1. Problems of Scope 系统的边界
2. Problems of Understanding 对目标的进一步理解
3. Problems of Volatility 需求的易变性（在敏捷开发中容易解决，不能解决的放下，下个迭代时再做）

### 8.1.2※ Eliciting Requirements诱导

和考试最后的35分综合题有关

简述：理解需求，构建用例图，类图等

**A. Collaborative协作的 Requirements Gathering 收集需求**

甲方最好提供1-2页的产品需求product request(在inception阶段）

基于产品需求开一次会，产品需求下发给每个参会者

会前每个参会者需制作的（需求调研要关注的4个方面）：

1. a list of **objects**找到名词或名词词组(也就是一大堆的object，比如属性，方法，常量等）；这些名词和词组有意义的找出来，数据有用的找出来来 (可能自己是对象；可能是某些名词的其他属性，这点要做好归属)
2. a list of **services(processes or functions)** 把（实意）动词（服务/功能）找出来
3. a list of **constraints** 找到约束和限制（比如：项目成本约束，项目大小约束，业务规则的约束）
4. **performance** & **nonfunctional** requirements 性能和系统的非功能需求

e.g：案例——HomeSafe系统

<img src="./\Chapter 8 需求 Requirements\Untitled 3.png" style="zoom:50%;" />

<img src="./\Chapter 8 需求 Requirements\Untitled 4.png" alt="Untitled 4" style="zoom:50%;" />

B. Quality Function Deployment（不重要）

**C. Usage Scenarios 开发一个初步的(preliminary)使用场景**

这一步开始初步开发Use Case Diagram，主要还是scenario层面。更细致地讨论在elaboration

<img src="./\Chapter 8 需求 Requirements\Untitled 5.png" style="zoom: 33%;" />

**D. Elicitation Work Products 得到需求规约草稿specification draft** 

需求诱导得到的work product会被系统的大小和软件产品的性质而影响变化

work product一般内容：

(1) a statement of need and feasibility, (2) a bounded statement of scope for the system or product, (3) a list of customers, users, and other stakeholders who participated in requirements elicitation, (4) a description of the system’s technical environment, (5) a list of requirements (preferably organized by function) and the domain constraints that applies to each, (6) a set of usage scenarios that provide insight into the use of the system or product under different operating conditions, and (7) any prototypes developed to better define requirements（理解）

（1） 需求和可行性声明，（2）系统或产品范围的有限声明，（3）参与需求激发的客户、用户和其他利益相关者的列表，（4）系统技术环境的描述，（5）需求列表（最好按功能组织）和适用于每个需求的领域约束，（6）一组使用场景，可深入了解系统或产品在不同操作条件下的使用情况（7）为更好地定义需求而开发的任何原型

E. Agile Requirements Elicitation（不强调）

F. Service-Oriented Methods（没啥）

### 8.1.3 Elaboration细化建模(ch11)

描述：把前两个阶段得到的信息进一步拓展和细化

步骤：

1. 进一步明确需求
2. 在这个基础上进行requirement analysis modeling

**对UseCase的进一步细化开发**

8章附件1-图书馆借阅管理系统.doc

**建立分析模型**

分析模型的组成元素：

1. 基于场景的元素：use case diagram ， activity diagram (功能建模）
2. 基于类的元素：class diagram （数据建模）
3. 行为模型： state diagram (sequence diagram)（行为建模）

(对于所有Use Case要构建sequence diagram，一个Use Case可以有多个sequence diagram)

功能建模，对每个功能逻辑的描述(在这里针对Use Case）

class diagram针对每一个use case把它的类提取出来，提取出来后是离散的，这里会引入泛化（ 把名词和名词词组总结出来后，看看名词有没有超过一个以上的属性值，则把它作为潜在的类；如果名词一个属性都没有，它可能是其他类的属性 ；再看有无继承、组合关系提取出来，最后将关系关联起来（一对一，一对多，多对多）

事件驱动：系统级别在事件驱动的状态下，可能有系统级别的状态图,是软件的体系结构设计依据。e.x. ATM机，在事件驱动下（读卡，输密码，选择交易，打印票据...靠外部动作事件驱动）--->是系统级别的状态图(state diagram），是main主程序设计的重要依据，而状态细节由客户定义出来。

大多数系统都没有系统级别的state diagram，但任何系统都有class的state diagram （把class diagram出来后，对每个class绘制状态图）

状态图（初步）：可能还要考虑很多collaborator,在设计迭代时完善

### 8.1.4 Negotiation 谈判

描述：做哪些，不做哪些，并确定优先级

①识别系统或子系统的主要利益相关者。---->谁是stakeholder

②确定利益攸关方的“胜利条件”。--->stakeholder怎么获利

③利益攸关方的赢得条件将其与所有有关的一系列单胜条件（包括软件团队）的胜利条件调和→调和

### 8.1.5 Specification 规约

描述：这一步形成2个规约文档

### 8.1.6 Validation 验证评审

描述：把模糊的ambiguously，省略的omission，错误的error部分改正

### 8.1.7 Requirement Management 需求管理
