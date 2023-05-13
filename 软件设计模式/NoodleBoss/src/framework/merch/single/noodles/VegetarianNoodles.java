package framework.merch.single.noodles;

import framework.ingredient.IngredientType;

import java.util.ArrayList;

public class VegetarianNoodles extends Noodles {
    public VegetarianNoodles(ArrayList<IngredientType> requirements, double price) {
        super(requirements, price);
    }

    @Override
    public String getName() {
        return "素面";
    }
}
