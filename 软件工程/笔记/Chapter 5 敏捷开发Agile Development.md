# Chapter 5 敏捷开发Agile Development

一种传统软件工程项目的合理的替代品

简述：敏捷软件工程可以快速提供成功的系统（重要特性：**适应需求变更**）

强调：

- 注重”客户的满意度“和”尽早提交可增量的软件产品“（增量交付，开发过程存在迭代）
- 快速交付，不看重中间产品
- 小型，积极的项目团队，协调能力强，看中团队结构，协作态度
- 可用非正式的方法（e.g. 数据库设计没有写完整的设计文档，中间建模尽量简单）
- 最小的work product（开发过程简单，会要求尽量减少work product且work product简单，不完全按照template）
- 整体发展简单（e.g. 封闭开发）
- 软件工程师和其他项目利益相关者（管理人员，客户，最终用户）在一个敏捷的团队，命运共同体，其中roles和responsibility随时调整

特点：

1. story / function： 通过需求调研得到项目的一些功能(story / functions)
2. priority：对story设置优先级
   
    在这之后，选择优先级高的stories进行第一次迭代，得到可演示产品；demo后进行第二次迭代，包括对上一次的stories进行修改，加入新的stories,......
    
3. 每次迭代完后均是可以演示运行的产品

基本框架活动 CPMCD (generic process,当然也可以遵循standard process)

## 5.1 Agility 敏捷

正在构建的软件的变化，对团队成员进行更改，由于新技术而更改，可能对其构建产品或创建产品的项目可能产生影响的各种类型。

## 5.2 Agility 和 Change Cost 改变成本

※ Change Cost图(出选择题，要理解）

![](./\Chapter 5 敏捷开发Agile Development\Untitled.png)

理解传统software processes的黑色曲线：

比较显然，随着软件开发逐步变化，需求变更可能需要改的部分越多（例如：还在modelling阶段和coding阶段change cost完全不同，coding阶段cost会大得多）

理解敏捷模型的曲线：

regression testing回归测试——变更多了后，回归测试成本越来越高，最终翘了上去（这一版和上一版本的修改会有牵连——相关用例）

![](./\Chapter 5 敏捷开发Agile Development\Untitled 1.png)

## 5.3 敏捷过程(Agile Process)

敏捷开发解决的问题：

1. 没有办法提前预测需求是什么，会怎么改，优先级如何
2. design和construction有交叉，过程中的activity是串联的。这样比较难去预测多少design在construction之前是必要的
3. 分析analysis, 设计design, 构建construction和测试testing和预想的不同

敏捷过程的目的：创建一个可以对付不可预知性的process(to rapidly changing project)

结论：需要一个增量的开发战略increment development strategy

### 5.3.1 敏捷原则Principles (12个)

※ 考试会出选择题（加混淆的选项）

1. **Highest** 最高优先级：通过早期early和连续continuous交付有价值的软件来满足客户
2. **Change** 鼓励需求改动，哪怕是开发过程晚期（好的改动可以提升顾客的竞争优势）
3. **Frequently** 经常提供work product，最好有短的schedule
4. **Together** 甲方和开发者daily紧密合作
5. **Motivated** 和有动力的人开发，给他们好的环境和支持，信任他们
6. **F-to-F** 面对面讨论，实时解决问题，这是最高效率的
7. **WorkingSoftware** 进度的最好指标就是产出的可工作的软件
8. **Sustainable** 敏捷过程保持稳定的开发过程，赞助人，开发者和用户应该maintain a constant pace indefinitely (不定期同步跟踪）
9. **Excellence** 持续关注追求技术的改进和设计的优化
10. **Simplicity** 文档尽量简单
11. **Self-organizing** 一个self-organizing的team可以做出最好的架构、需求和设计
12. **Reflection** 定期进行回顾

## 5.4 极限编程Extreme Programming

### 5.4.1 XP Process

是agile  model

是迭代开发的，每轮结束后都可以运行的系统

4个framework activities：Planning，Design，Coding和Testing（区别Generic Process Activity的Framework(CPMCD)

![](./\Chapter 5 敏捷开发Agile Development\Untitled 2.png)

- Planning: 导出需求
    1. 设定一系列stories，故事的单位是use case
    2. 客户基于特征或功能的整体业务价值为story分配优先级，划分依据：
       
        业务上重要性
        
        此功能(story)是否有高风险(high risk)
        
        交付时间(deadline要求）
    3. 如果估计故事需要超过三个发展周(工作量太大），请客户拆分为较小的故事
    4. 一旦基本承诺定下（关于要包括故事的协议agreement，交货日期和其他项目问题），XP团队会将以三种方式之一开发story：
       
        所有story将被实施立即（几周之内）
        
        最高优先级的story将在计划中提前并首先实施
        
        最高风险的story将在计划中提前并首先实施
    5. 迭代
    
- Design：
    - 鼓励使用CRC模型(Chapter 10 )
    - **Spike Solution**——快速解决问题的模型一种Design Prototype，对没有把握的部分先给出一个快速解决方案（原型）在特定的环境下跑一下（有时不知道是否正确，时间复杂度等也未知，此时可以先快速开发，看有没有把握去解决问题）

- Coding

    设计完成后，团队不会转移到代码，而是开发一系列unit test，该测试将测试当前版本中包含的每个story（软件增量software increment）：业务流程的梳理

    1. Pair Programming 结对编程（互相看逻辑做Check）
    2. Refactoring 重构（对代码规范化，标准化以进行复用）
    3. Unit test
       
        在programming前首先将单元测试的测试用例写出来，用test case覆盖逻辑
        
        pair programming后马上就运行unit test的测试用例（测试先行）
        
        - Continuous integration  2个人结对编程时把两个人的代码做基础

    （注意！continuous integration的目的是把两个人的代码做集成；而test中的continuous integration是把不同的pair program写的代码做集成）

- Test
    - Continuous integration 将pair program结对编程小组的代码整合
    - Unit test 需重新运行coding阶段的test case；再加一些test case（集成中发现的问题，因为一般一些测试类用例是优化顺序的
    - 做成一个test suite测试套件，生成test case set测试集正常执行
    - acceptance test: 一般是开发人员模拟用户将story的逻辑全部测试一遍

- Release:

    software increment project velocity要计算，必入衡量完成的占比

    第一个project（software increment）在发布后，XP team计算目前完成的速率(project velocity)——第一个release版本中部署的customer stories的数量。它可以用于评估接下来release的发布日期和开发schedule，以及看整个项目的所有stories是否存在overcommitment(过度承诺)，如果有，则要修改release内容或推迟delivery dates

    敏捷原则：
    
    1. 个体与交互 胜过 过程与工具
    2. 可以工作的软件 胜过 面面俱到的文档
    3. 重视客户协作（每次迭代的完成）胜过 合同谈判
    4. 响应变化 胜过 遵循计划

    ## 5.6 SCRUM

    一种敏捷开发框架，是增量迭代的开发过程（很重要，考试必考SCRUM或XP）

    **阅读公盘的课外材料**

    ## 5.7DevOps:

    ※**DevOps: 现在开发常用的开发框架**

     软件开发人员(Dev)和订正人员(Ops)之间的沟通合作

    DevOps由Patrick DeBois创建，旨在将开发和运营结合起来。DevOps试图在整个软件供应链中应用敏捷和精益开发原则。图5.3概述了DevOps工作流。DevOps方法涉及几个阶段，这些阶段不断循环，直到出现所需的产品：

    持续发展。软件可交付成果被分解并在多个sprint中开发，增量交付给开发团队的质量保证成员进行测试。

    连续测试。自动化测试工具用于帮助团队成员同时测试多个代码增量，以确保它们在集成之前没有缺陷。

    持续集成。具有新功能的代码段将添加到现有代码和运行时环境中，然后检查以确保部署后没有错误。

    持续部署。在这个阶段，集成的代码被部署（安装）到生产环境中，其中可能包括需要准备好接收新功能的全球多个站点。

    持续监测。操作(运维) 作为开发团队成员的工作人员通过监控软件在生产环境中的性能并在用户发现问题之前主动查找可能的问题，帮助提高软件质量
    
    ![](./\Chapter 5 敏捷开发Agile Development\Chapter_05-new.jpg)

DevOps通过快速响应客户需求或愿望的变化来增强客户体验。这可以提高品牌忠诚度和市场份额。像DevOps这样的精益方法可以通过减少返工并允许转向更高业务价值的活动，为组织提供更高的创新能力。在消费者能够使用产品之前，产品不会赚钱，DevOps可以为生产平台提供更快的部署时间