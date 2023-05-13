package framework.ingredient.state;

import framework.ingredient.Ingredient;

public class IngredientFreshState implements IngredientState {
    @Override
    public boolean isStale(Ingredient ingredient) {
        return false;
    }

    @Override
    public boolean isCooked(Ingredient ingredient) {
        return false;
    }
}
