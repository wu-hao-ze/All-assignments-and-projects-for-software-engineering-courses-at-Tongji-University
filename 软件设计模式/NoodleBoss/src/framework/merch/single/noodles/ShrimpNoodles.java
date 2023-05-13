package framework.merch.single.noodles;

import framework.ingredient.IngredientType;

import java.util.ArrayList;

public class ShrimpNoodles extends Noodles {
    public ShrimpNoodles(ArrayList<IngredientType> requirements, double price) {
        super(requirements, price);
    }

    @Override
    public String getName() {
        return "虾仁面";
    }

}
