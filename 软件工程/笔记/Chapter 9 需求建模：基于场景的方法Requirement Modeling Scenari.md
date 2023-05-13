# Chapter 9 需求建模：基于场景的方法Requirement Modeling:Scenario-Based Methods

Use case/Activity/Swim lane用例图/活动图/泳道图

## 9.1 需求分析Requirement Analysis

需求建模这个Action会产出以下一个或多个种类的模型：

1. Scenario-Based Models  基于场景的需求模型（从多个系统的"actors"为出发点）**Chapter 9**
2. Class-Oriented Models or Data Models 类继承的模型或数据模型（基于信息域，表示面向对象类（属性&方法）和类之间的协作collaboration (数据模型现在也基本不用了) **Chapter 10**
3. Behavior Models 行为模型（外部的"event"引起的软件的动作）**Chapter 11**
4. Functional Models/Flow-Oriented Models 基于流的建模（现在基本不用）

### 9.1.1 需求建模的目标和学问Overall Objectives and Philosophy

※可能考多选题

需求建模的3个目的(primary objectives of the requirement model)：

1. 描述用户想要什么
2. 为设计提供基础(概要设计&组件设计）
3. 定义需求的集合
   
    > 代码写完后，这个会作为评审的依据   e.g. 系统测试的依据→需求分析，需求分析规约
    > 

重要的： 需求模型中的所有元素都对应了设计模型(design model)的一部分。但在这两个重要的建模活动之间清晰的analysis task和design task划分并不总是可能的

<img src="./\Chapter 9 需求建模：基于场景的方法Requirement Modeling Scenari\Untitled.png" style="zoom:50%;" />

（这个边界要根据项目自身的情况确定）

### 9.1.2 分析建模的经验规则 Analysis Rules of Thumb

分析建模需要遵循的一些规则：

1. **Abstraction&Visible**抽象化程度要高，需求应该在问题和业务域内可视化(visible)
2. **Element**需求模型的每个元素都应增加对软件需求的整体理解，并提供系统的信息域，功能和行为
3. **Delay**做分析建模时可以先不考虑
   
    1. 更细节的基础物件（e.g：数据库设计，前端界面等）
    2. 非功能需求和性能的建模细化（e.g 架构，算法，数据库设计等影响性能的因素）
4. **Min-Coupling** 低耦合
   
    > 在构建UseCaseDiagram时，需要注意每个UseCase尽量高内聚，状态图中也有体现（大状态分为多个子状态（粒度小，内聚性高），只有高内聚后才有低耦合
    > 
5. **Value** 要构建对Stakeholder有价值的需求模型
6. **Simple** 业务简单化

### 9.1.3 领域分析Domain Analysis

领域分析目的(为什么要Domain Analysis）：把一些公共的analysis class和analysis pattern提取出来，对公共类进行很好定义再标化，以便于以后使用

(e.g: 政府电子化办公，做完后进行很多类的标准化，**形成类库，**便于以后使用，避免重复开发）

领域分析可能被视为一个umbrella activity，是一个持续的软件工程activity

领域分析师：发现并定义一些很多人使用（不一定在一个应用中）分析模式，分析类（公共）和互相有联系的信息

※构建可复用类库的方式：

<img src="./\Chapter 9 需求建模：基于场景的方法Requirement Modeling Scenari\Untitled 1.png" style="zoom:67%;" />

### 9.1.4 需求模型方法 Requirements Modeling Approaches

Give a example for structured analysis modeling 9章附件 DFD data flow diagram建模.doc

※分析模型的元素

<img src="./\Chapter 9 需求建模：基于场景的方法Requirement Modeling Scenari\Untitled 2.png" style="zoom:50%;" />

## 9.2 SCENARIO-BASED MODELING

（好像不大重要）

## 9.3 基于UseCase的UML模型 UML Models

UML活动图通过提供特定方案内的交互流的图形表示来补充用例

### 9.3.1 活动图制作Activity Diagram

活动图需要针对每一个use case

对一个UseCase

- 数据建模 Class Diagram 语法扫描——名词或名词词组可能是潜在业务类(Chapter 8)
    - 要对项目所提出的所有class做聚合和泛化（继承，目的是为了复用）
- 功能建模 Activity Diagram(√ ）
- 行为建模 State Diagram，Sequence Diagram

<img src="./\Chapter 9 需求建模：基于场景的方法Requirement Modeling Scenari\Untitled 3.png" style="zoom:67%;" />

### 9.3.2 泳道图制作 Swim Lane Diagram

泳道图是需要类的参与的，activity需要哪些类交互完成功能，在这个基础上完成了swimlane，所以一般在activity diagram和类图构建完之后再回到use case图构建swimlane

构建activity diagram要注意有正常异常等业务逻辑，所有业务逻辑的出口和流程都要在activity diagram表现；activity diagram也是构建sequence diagram的依据

<img src="./\Chapter 9 需求建模：基于场景的方法Requirement Modeling Scenari\Untitled 4.png" style="zoom:50%;" />