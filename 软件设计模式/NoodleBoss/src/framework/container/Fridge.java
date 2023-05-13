package framework.container;

import framework.chef.ChefVisitor;
import framework.ingredient.IngredientType;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * DP:
 * Singleton, Visitor
 */
public class Fridge extends Container {

    private Fridge() {
        super(new ArrayList<>(Arrays.asList(
                IngredientType.SHRIMP,
                IngredientType.PORKPATTY)));

        if (instance == null) {
            instance = this;
        } else {
            throw new IllegalStateException("Already initialized.");
        }
    }

    private static Fridge instance;

    public static synchronized Fridge getInstance() {
        if (instance == null) {
            instance = new Fridge();
        }
        return instance;
    }
    public String getName() {
        return "冰箱";
    }
    @Override
    public void accept(ChefVisitor visitor) {
        visitor.visit(this);
    }
}

