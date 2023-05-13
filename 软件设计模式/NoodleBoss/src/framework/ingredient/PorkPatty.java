package framework.ingredient;

import framework.cooker.Hearth;

public class PorkPatty extends Ingredient {
    PorkPatty(double baseStateUpdateRate) {
        super(baseStateUpdateRate);
        this.setCooker(new Hearth());
    }

    @Override
    public IngredientType getIngredientType() {
        return IngredientType.PORKPATTY;
    }

    @Override
    public String getName() {
        return "大排";
    }
}
