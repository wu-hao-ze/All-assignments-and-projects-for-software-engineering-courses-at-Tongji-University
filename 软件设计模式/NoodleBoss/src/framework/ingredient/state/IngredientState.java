package framework.ingredient.state;

import framework.ingredient.Ingredient;

/**
 * DP: State
 */
public interface IngredientState {
    public abstract boolean isStale(Ingredient ingredient);

    public abstract boolean isCooked(Ingredient ingredient);
}
