package framework.merch.single.noodles;

import framework.ingredient.IngredientType;

import java.util.ArrayList;

public class PorkNoodles extends Noodles {
    public PorkNoodles(ArrayList<IngredientType> requirements, double price) {
        super(requirements, price);
    }

    @Override
    public String getName() {
        return "大排面";
    }
}
