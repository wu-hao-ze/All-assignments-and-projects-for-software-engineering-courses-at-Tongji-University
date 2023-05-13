package framework.ingredient;

import framework.cooker.CounterTop;

public class Pakchoi extends Ingredient {
    Pakchoi(double baseStateUpdateRate) {
        super(baseStateUpdateRate);
        this.setCooker(new CounterTop());
    }

    @Override
    public IngredientType getIngredientType() {
        return IngredientType.PAKCHOI;
    }

    @Override
    public String getName() {
        return "青菜";
    }
}
