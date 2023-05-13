package framework.cooker;

import framework.ingredient.Ingredient;
import framework.ingredient.IngredientType;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * DP: Template
 */
public abstract class Cooker {
    protected ArrayList<IngredientType> availableTypes = new ArrayList<IngredientType>();

    private boolean isIngredientTypeLegal(IngredientType type) {
        return availableTypes.contains(type);
    }

    public final void cook(Ingredient ingredient) {
        if (!(this.isIngredientTypeLegal(ingredient.getIngredientType()))) {
            throw new IllegalArgumentException("this ingredient doesn't belong here");
        }
        startCooking(ingredient);
        finishCooking(ingredient);
    }

    abstract void startCooking(Ingredient ingredient);

    abstract void finishCooking(Ingredient ingredient);

    public abstract String getName();
}
