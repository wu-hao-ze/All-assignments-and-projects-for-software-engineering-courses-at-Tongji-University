package framework.container;

import framework.chef.ChefVisitor;
import framework.ingredient.IngredientType;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * DP:
 * Singleton, Visitor
 */
public class Cabinet extends Container {

    private Cabinet() {
        super(new ArrayList<>(Arrays.asList(
                IngredientType.ROWNODDLE,
                IngredientType.PAKCHOI)));

        if (instance == null) {
            instance = this;
        } else {
            throw new IllegalStateException("Already initialized.");
        }

    }

    private static Cabinet instance;

    public static synchronized Cabinet getInstance() {
        if (instance == null) {
            instance = new Cabinet();
        }

        return instance;
    }

    public String getName() {
        return "橱柜";
    }

    @Override
    public void accept(ChefVisitor visitor) {
        visitor.visit(this);
    }

}
