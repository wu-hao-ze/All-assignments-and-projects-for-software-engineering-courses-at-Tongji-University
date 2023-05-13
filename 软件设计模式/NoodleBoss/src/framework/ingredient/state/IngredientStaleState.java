package framework.ingredient.state;

import framework.ingredient.Ingredient;

public class IngredientStaleState implements IngredientState {
    @Override
    public boolean isStale(Ingredient ingredient) {
        return true;
    }

    @Override
    public boolean isCooked(Ingredient ingredient) {
        return false;
    }
}
