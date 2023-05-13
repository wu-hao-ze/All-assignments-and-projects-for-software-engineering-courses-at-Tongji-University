package framework.ingredient.state;

import framework.ingredient.Ingredient;

public class IngredientCookedState implements IngredientState {
    @Override
    public boolean isCooked(Ingredient ingredient) {
        return true;
    }

    @Override
    public boolean isStale(Ingredient ingredient) {
        return false;
    }
}