package framework.ingredient;

import java.util.ArrayList;

/**
 * Factory Pattern
 */
public class IngredientFactory {
    public Ingredient createIngredient(IngredientType ingredientType) {
        switch (ingredientType) {
            case PORKPATTY:
                return new PorkPatty(0.5);
            case ROWNODDLE:
                return new Rownoodle(0.8);
            case PAKCHOI:
                return new Pakchoi(0.2);
            case SHRIMP:
                return new Shrimp(1.1);
            default:
                throw new IllegalArgumentException("no such ingredient");
        }
    }

    public ArrayList<Ingredient> createIngredientList(IngredientType ingredientType, Integer count) {
        ArrayList<Ingredient> ingredients = new ArrayList<>();
        for (Integer i = 0; i < count; i++) {
            ingredients.add(this.createIngredient(ingredientType));
        }
        return ingredients;
    }
}
