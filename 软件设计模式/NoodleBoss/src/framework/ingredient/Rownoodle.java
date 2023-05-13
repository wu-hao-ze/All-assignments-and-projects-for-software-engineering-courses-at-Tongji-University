package framework.ingredient;


import framework.cooker.CounterTop;

public class Rownoodle extends Ingredient {

    Rownoodle(double baseStateUpdateRate) {
        super(baseStateUpdateRate);
        this.setCooker(new CounterTop());
    }

    @Override
    public IngredientType getIngredientType() {
        return IngredientType.ROWNODDLE;
    }

    @Override
    public String getName() {
        return "面条";
    }
}
