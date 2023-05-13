# Chapter 3 软件过程Software Process

## 3.1 通用过程模型Generic Process Model

通用过程框架(generic process framework) 定义了5个框架活动(framework activity)——CPMCD，以及一系列伞活动(umbrella activity)——project tracking and control, risk management, quality assurance, configuration management, technical reviews, and others

**※软件过程框架Software process framework**

通用过程框架(generic process framework) 包含两部分

- 框架活动(framework activity)(CPMCD)——development dimension开发维度
- 伞型活动(umbrella activity)——management dimension管理维度(评审，QA等）

framework activity → software engineering action → task sets

action是若干个task的集合(sets)，其内容：

- T：work tasks:
- P：work products: 在work tasks之后形成一个work products
- Q：quality assurance( QA ) points: 产生work products后要有质量保证
- M：milestone: 形成里程碑

![](./\Chapter 3 软件过程Software Process\Untitled.png)

**※工作流Process flow**

process flow过程流程，工作流 - 描述了如何在序列和时间内组织在每个框架活动中发生的框架活动的动作和任务

种类:

1. Linear 线性
2. Iterative 迭代
3. Evolutionary 演化
4. Parallel 并行
- **线性工作流和迭代工作流**

![](./\Chapter 3 软件过程Software Process\Untitled 1.png)

迭代流的几个迭代箭头：

1. communication和planning反复迭代(planning后，由于1.需求变更;2. 技术和业务更深层的理解;3. 有错误等，重复迭代)
2. modeling部分的迭代
3. construction后(coding&testing)，可能在testing时发现需求不明确，又回到了前面四个阶段迭代
- **演化工作流(Evolutionary)——螺旋上升**

![](./\Chapter 3 软件过程Software Process\Untitled 2.png)

解读：本质还是迭代开发

Planning不回到Communication: 在planning阶段发现有communication的问题，但不回去，而是记录下来直到把本次迭代做完，不论有多少问题，都先进行完本次开发

increment released: **和单纯的迭代流不同，每次迭代完必须要有一个可展示的产品**

每一次的输入（下次迭代要解决的问题）：下次开发的新功能+本次开发中记录下来的问题+用户使用这个increment的产品后反馈要解决的问题

- **并行工作流(Parallel)**

![](./\Chapter 3 软件过程Software Process\Untitled 3.png)

planning和modeling同时做，在communication时导出需求，可以进行planning；同时明确的部分可以进行modeling

## 3.2 定义框架活动Framework Activity

框架活动的关键问题：考虑到要解决的问题的性质、从事工作的人员的特征以及赞助项目的利益相关者，哪些行动适合框架活动？

- 小项目：

task：

1. 和stakeholder（利益相关者，股东）通过电话交流；
2. 将笔记组织成一个简单的需求书面(written)声明；
3. 和stakeholder用电子邮件进行审阅和通过

- 大项目：

action:

1. inception 
2. elicitation (诱导需求，功能需求和非功能需求function requirement& non-function requirements)→requirement gathering
3. elaboration (建模，用UML各种diagram对需求内容进行抽象)
4. negotiation ( 甲方讨论需要和不需要的需求，需求规约，需求分析) 
5. specification 
6. validation

开发过程控制:MS-project

## 3.3 定义任务列表Identify a Task Set

- 软件工程工作任务work tasks
- 相关工作产品work products
- 质量保证点 quality assurance points
- 项目里程碑project milestone的集合

例子：

elicitation:

![](./\Chapter 3 软件过程Software Process\Untitled 4.png)

e.g. 书上没有的例子:

Coding过程的Action :

task set: 

1. 分析理解详细设计（详细的设计规约，理解接口）；
2. 算法以及数据结构；
3. 准备环境写代码
4. (单元测试）自我测试self-testing（可能考虑的：算法复杂度时间性能）；
5. SQA软件质量保证；
6. refactor重构（测试后对大项目可能要重构，规范性的）
7. code review；
8. code end(里程碑任务milestone)

提高系统性能的几个方法：

1. 后端的数据库设计(冗余设计，表空间划分，范式)(高访问量的几张表不能放在一个表空间里)；
2. 架构设计architecture design；
3. 前端和后端的接口（交易频繁的分到不同接口支流中）；
4. 前端本身的memory使用，和后端的通讯；
5. 模拟工具测试得到性能

## 3.4 过程模式 Process Pattern

**※ 非常重要**

定义：

过程模式Process Pattern描述了在软件工程工作中遇到的与过程有关的问题，确定了遇到该问题的环境，并提出了一个或多个行之有效的解决问题的方法。

**※ 描述模板template**（主要看例子）

| 英文 | 中文 | 描述 | 例子 |
| --- | --- | --- | --- |
| Pattern Name | 模式名字 | 形容软件过程的context | TechnicalReviews |
| Force | 环境 | 所需要的环境，硬件，网络，版本管理工具等 |  |
| Type | 类型 | stage pattern: 解决activity相关的（EstablishingCommunication）；task pattern：解决action或task的(RequirementGathering)；phase pattern: 整个框架活动的序列，涉及各种activity(SpiralModel, Prototyping) | stage pattern; task pattern; phase pattern |
| Initial Context | 启动条件 | 在模式启动之前： (1) 已经发生了哪些组织或团队相关的活动？ (2)进程的入口状态是什么？ (3) 已有哪些软件工程信息或项目信息？ （Activity Happended? State ? SE info) | 规划模式Stage pattern的Initial Context（1）客户和软件工程师建立了协作沟通； (2) 成功完成了通信模式的多个任务模式[指定]； (3)项目范围、基本业务需求、项目约束条件已知。 |
| Problem | 问题 | Pattern可以用来解决什么问题 |  |
| Solution | 解决方案 | 描述如何成功执行Pattern |  |
| Resulting Context | 接口（出口条件） | 为接下来提交什么信息(哪些activity必须出现；过程的出口状态是怎样的；开发了什么软件工程信息) |  |
| Related Pattern | 相关的Pattern | 提供与此直接相关的所有流程模式的列表。这可以表示为层次结构或以其他示意性形式表示（同级或上下） | 比如：同一个action下的两个task上下相关；又比如关于unit test不知道怎么做，去寻找相关、包含或并行的stage pattern |
| Know Uses and Examples | 用过的案例 |  |  |

![](./\Chapter 3 软件过程Software Process\Untitled 5.png)

## 3.5 过程评估与改进Assessment and Improvement

Process patterns必须与可靠的软件工程实践相结合。 此外，可以对过程本身进行评估，以确保它满足一组基本过程标准，这些标准已被证明对于成功的软件工程至关重要。

(CMMI)

Initial Repeatable Defined Managed Optimize

## 软件成熟度模型CMMI

![](./\Chapter 3 软件过程Software Process\Untitled 6.png)	
