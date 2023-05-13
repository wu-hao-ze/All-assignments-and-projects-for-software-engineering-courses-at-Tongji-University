package framework.cooker;

import framework.ingredient.Ingredient;
import framework.ingredient.IngredientType;
import framework.ingredient.state.IngredientStateType;

import java.util.ArrayList;
import java.util.Arrays;

public class CounterTop extends Cooker {
    public CounterTop() {
        this.availableTypes = new ArrayList<>(Arrays.asList(
                IngredientType.ROWNODDLE,
                IngredientType.PAKCHOI));
    }

    @Override
    public String getName() {
        return "操作台";
    }

    @Override
    void startCooking(Ingredient ingredient) {
        System.out.println("开始使用操作台处理" + ingredient.getName() + "...");
    }

    @Override
    void finishCooking(Ingredient ingredient) {
        ingredient.changeState(IngredientStateType.COOKED);
        System.out.println("你成功地使用" + this.getName() + "加工好了" + ingredient.getName());
    }
}
