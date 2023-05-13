
import framework.chef.Chef;
import framework.chef.SimpleChefVisitor;
import framework.container.*;
import framework.cooker.CounterTop;
import framework.cooker.Hearth;
import framework.ingredient.Ingredient;
import framework.ingredient.IngredientFactory;
import framework.ingredient.IngredientMemento;
import framework.ingredient.IngredientType;
import framework.ingredient.state.IngredientStateType;
import framework.ingredient.state.IngredientStateFactory;
import framework.merch.Merch;
import framework.merch.MerchFactory;
import framework.merch.MerchFactoryMaker;
import framework.merch.MerchType;
import framework.merch.set.Set;
import framework.order.Order;
import framework.order.OrderBuilder;
import framework.time.Timer;

public class Main {

    public static void main(String[] args) {
        Main.TestServeOrder();
        Main.TestSingleton();
        Main.TestAbstractFactory();
        Main.TestFactory();
        Main.TestTemplate();
        Main.TestObserver();
        Main.TestState();
        Main.TestDecorator();
        Main.TestStrategy();
        Main.TestBuilder();
        Main.TestBridge();
        Main.TestFacade();
        Main.TestFlyweight();
        Main.TestPrototype();
        Main.TestNullObject();
        Main.TestComposite();
        Main.TestVisitor();
        Main.TestMemento();
        Main.TestIterator();

    }

    public static void TestServeOrder() {
        // 下单测试
        // 单例模式 Singleton
        Chef chef = Chef.getInstance();

        // Command && Facade
        chef.showStock();

        System.out.println("内部时间流动 10 个 tick，食材的新鲜度发生变化！");
        for (int i = 0; i < 10; i++) {
            Timer.getInstance().tick();
        }
        chef.showStock();

        // Builder
        OrderBuilder builder = new OrderBuilder();
        builder.orderNoodles(MerchType.PORK_NOODLES);
        // Composite
        builder.orderSet(MerchType.SET_PORK_NOODLES_COKE);
        builder.orderBeverage(MerchType.COKE);
        Order order = builder.order();

        // Command && Facade
        chef.processOrder(order);
    }

    public static void TestSingleton() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        System.out.printf("比较大厨实例地址是否相等: %b\n", Chef.getInstance() == Chef.getInstance());
        System.out.printf("比较橱柜实例地址是否相等: %b\n", Cabinet.getInstance() == Cabinet.getInstance());
        System.out.printf("比较冰箱实例地址是否相等: %b\n", Fridge.getInstance() == Fridge.getInstance());
        System.out.println("==================");
    }

    public static void TestAbstractFactory() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        MerchFactory factory1 = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.SET);
        MerchFactory factory2 = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.NOODLES);
        System.out.printf("使用抽象工厂创建一个套餐工厂: %s; 再使用该工厂创建商品: %s\n", factory1.getClass().getCanonicalName(), factory1.createSet(MerchType.SET_PORK_NOODLES_COKE).getName());
        System.out.printf("使用抽象工厂创建一个套餐工厂: %s\n", factory2.getClass().getCanonicalName());

        System.out.println("==================");
    }

    public static void TestFactory() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        IngredientFactory factory = new IngredientFactory();
        System.out.printf("创建食材工厂: %s\n", factory.getClass().getCanonicalName());
        System.out.printf("创建食材: %s\n", factory.createIngredient(IngredientType.PORKPATTY).getName());
        System.out.println("==================");
    }

    public static void TestTemplate() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Ingredient pork = new IngredientFactory().createIngredient(IngredientType.PORKPATTY);
        Ingredient pakchoi = new IngredientFactory().createIngredient(IngredientType.PAKCHOI);
        new Hearth().cook(pork);
        new CounterTop().cook(pakchoi);
        System.out.println("==================");
    }

    public static void TestObserver() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Ingredient pork = new IngredientFactory().createIngredient(IngredientType.PORKPATTY);
        pork.displayInfo();
        Timer.getInstance().tick();
        pork.displayInfo();
        System.out.println("==================");
    }

    public static void TestState() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Ingredient pork = new IngredientFactory().createIngredient(IngredientType.PORKPATTY);
        System.out.println("> 初始状态");
        System.out.printf("猪排是否加工： %b；腐烂: %b\n", pork.isCooked(), pork.isStale());
        System.out.println("> 状态->加工");
        pork.changeState(IngredientStateType.COOKED);
        System.out.printf("猪排是否加工： %b；腐烂: %b\n", pork.isCooked(), pork.isStale());
        System.out.println("> 状态->腐烂");
        pork.changeState(IngredientStateType.STALE);
        System.out.printf("猪排是否加工： %b；腐烂: %b\n", pork.isCooked(), pork.isStale());
        System.out.println("==================");
    }

    public static void TestDecorator() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        OrderBuilder builder = new OrderBuilder();
        builder.orderBeverage(MerchType.COKE);
        Order order = builder.order();

        order.displayMerches();

        TrayDecorator decorator = new TrayDecorator(order);
        System.out.println("> 经过 TrayDecorator 后");
        decorator.displayMerches();
        System.out.println("==================");
    }

    public static void TestStrategy() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Ingredient pork = new IngredientFactory().createIngredient(IngredientType.PORKPATTY);
        System.out.println("> 设置牛肉的加工工具为 烤架");
        pork.setCooker(new Hearth());
        pork.doCook();
        System.out.println("==================");
    }

    public static void TestBuilder() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        OrderBuilder builder = new OrderBuilder();
        builder.orderNoodles(MerchType.PORK_NOODLES);
        builder.orderSet(MerchType.SET_PORK_NOODLES_COKE);
        builder.orderSet(MerchType.SET_PORK_NOODLES_COKE);
        builder.orderBeverage(MerchType.COKE);
        Order order = builder.order();

        order.displayMerches();
        System.out.println("==================");
    }

    public static void TestBridge() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        OrderBuilder builder = new OrderBuilder();
        builder.orderNoodles(MerchType.PORK_NOODLES);
        builder.orderSet(MerchType.SET_PORK_NOODLES_COKE);
        builder.orderBeverage(MerchType.COKE);
        Order order = builder.order();
        System.out.printf("订单价格，通过内部商品计算 %f\n", order.totalPrice());
        System.out.println("==================");
    }

    public static void TestFacade() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        System.out.println("> 厨师的操作对外简化成几个简单的接口，内部操作对外隐藏");
        Chef chef = Chef.getInstance();

        chef.showStock();
        System.out.println("==================");
    }

    public static void TestFlyweight() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        System.out.println("> 食材状态对象全局共享");
        System.out.printf("fresh: %b\n", IngredientStateFactory.getState(IngredientStateType.FRESH) == IngredientStateFactory.getState(IngredientStateType.FRESH));
        System.out.println("==================");
    }

    public static void TestPrototype() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Ingredient pork = new IngredientFactory().createIngredient(IngredientType.PORKPATTY);
        try {
            Ingredient pork2 = (Ingredient) pork.clone();
            System.out.printf("pork1==pork2: %b\n", pork == pork2);
            System.out.println("> pork1 info");
            pork.displayInfo();
            System.out.println("> pork2 info");
            pork2.displayInfo();
        } catch (CloneNotSupportedException e) {
            System.out.println("Clone is not supported on this ingredient");
        }
        System.out.println("==================");
    }

    public static void TestNullObject() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        System.out.println("> 尝试创建一个不存在的工厂");
        MerchFactory factory1 = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.NULL);
        Merch merch = factory1.createSet(MerchType.SET_PORK_NOODLES_COKE);
        System.out.printf("对象: %s, 价格: %f\n", merch.getClass().getCanonicalName(), merch.getPrice());
        System.out.println("==================");
    }

    public static void TestComposite() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        MerchFactory factory1 = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.SET);
        Set set = factory1.createSet(MerchType.SET_PORK_NOODLES_COKE);
        System.out.printf("套餐名：%s, 套餐价格：%s\n", set.getName(), set.getPrice());
        System.out.println("==================");
    }

    public static void TestVisitor() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Cabinet.getInstance().accept(new SimpleChefVisitor());
        System.out.println("==================");
    }

    public static void TestMemento() {
        System.out.println("==================");
        System.out.println(Thread.currentThread().getStackTrace()[1].getMethodName());
        Ingredient ingredient = new IngredientFactory().createIngredient(IngredientType.PORKPATTY);
        ingredient.displayInfo();
        // 保存状态
        IngredientMemento memento = ingredient.getMemento();

        System.out.println("1000 tick");
        for (int i = 0; i < 1000; ++i) {
            Timer.getInstance().tick();
        }
        ingredient.displayInfo();

        System.out.println("复原");
        ingredient.setMemento(memento);
        ingredient.displayInfo();
        System.out.println("==================");
    }

    public static void TestIterator() {
        Fridge fridge = Fridge.getInstance();
        System.out.println("冰箱里有：");

        for (ContainerIterator iterator = fridge.Iterator(); iterator.hasNext(); ) {
            Ingredient ingredient = iterator.next();
            System.out.printf("* %s\n", ingredient.getName());
        }
    }
}
