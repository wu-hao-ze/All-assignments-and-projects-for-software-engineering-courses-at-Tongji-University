package framework.ingredient;

import framework.cooker.Hearth;

public class Shrimp extends Ingredient {
    Shrimp(double baseStateUpdateRate) {
        super(baseStateUpdateRate);
        this.setCooker(new Hearth());
    }

    @Override
    public IngredientType getIngredientType() {
        return IngredientType.SHRIMP;
    }

    @Override
    public String getName() {
        return "虾仁";
    }
}
