package framework.chef;

import framework.container.Cabinet;
import framework.container.Fridge;
import framework.container.TrayDecorator;
import framework.ingredient.Ingredient;
import framework.ingredient.IngredientType;
import framework.order.Order;

/**
 * DP:
 * Singleton
 */
public final class Chef implements ChefCommand {
    private Chef() {
        if (instance == null) {
            instance = this;
        } else {
            throw new IllegalStateException("Already initialized.");
        }

        System.out.println("厨子，接下来要开始干活了！\n");
    }

    private static Chef instance;

    private ChefVisitor visitor = new SimpleChefVisitor();

    public static synchronized Chef getInstance() {
        if (instance == null) {
            instance = new Chef();
        }
        return instance;
    }

    @Override
    public void showStock() {
        System.out.println("查看库存中...");
        Cabinet cabinet = Cabinet.getInstance();
        Fridge fridge = Fridge.getInstance();
        System.out.println("这就是厨房里所有的容器了！\n");
        cabinet.accept(visitor);
        fridge.accept(visitor);
    }

    public boolean hasIngredient(IngredientType type) {
        if (type == IngredientType.ROWNODDLE || type == IngredientType.PAKCHOI) {
            return Cabinet.getInstance().has(type);
        } else {
            return Fridge.getInstance().has(type);
        }
    }

    public Ingredient getIngredient(IngredientType type) {
        if (type == IngredientType.ROWNODDLE || type == IngredientType.PAKCHOI) {
            return Cabinet.getInstance().fetch(type);
        } else {
            return Fridge.getInstance().fetch(type);
        }
    }

    /**
     * DP：Facade
     * 对外只提供处理订单接口，内部实现处理的逻辑是取食材、加工、整合。。。
     * @param order
     */
    @Override
    public void processOrder(Order order) {
        System.out.println("厨子，刚刚接到新订单！");
        order.displayMerches();
        order.handle();
        TrayDecorator decorator = new TrayDecorator(order);
        decorator.displayMerches();
        System.out.println("\n订单完成！");
    }
}
