package framework.merch.single.noodles;

import framework.chef.Chef;
import framework.ingredient.Ingredient;
import framework.ingredient.IngredientType;
import framework.merch.single.SingleOrder;

import java.util.ArrayList;

public abstract class Noodles extends SingleOrder {
    private ArrayList<IngredientType> requirements = new ArrayList<>();

    Noodles(ArrayList<IngredientType> requirements, double price) {
        super(price);
        this.requirements.addAll(requirements);
    }

    public ArrayList<IngredientType> getRequirements() {
        return requirements;
    }

    @Override
    public void handle() {
        System.out.println("\n开始加工" + this.getName());
        Chef chef = Chef.getInstance();

        for (IngredientType type : requirements) {
            if (!(chef.hasIngredient(type))) {
                System.out.println("缺少材料，加工失败：(");
                return;
            }
        }

        for (IngredientType type : requirements) {
            Ingredient ingredient = chef.getIngredient(type);
            ingredient.doCook();
        }

        System.out.println("哇哇哇！你成功地做好了" + this.getName());
    }
}
