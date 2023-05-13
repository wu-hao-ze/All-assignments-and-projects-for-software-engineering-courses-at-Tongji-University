package framework.chef;

import framework.container.Cabinet;
import framework.container.Container;
import framework.ingredient.Ingredient;

/**
 * Visitor
 */
public class SimpleChefVisitor implements ChefVisitor {
    @Override
    public void visit(Container container) {
        System.out.println(container.getName() + "里存储着：");
        for (Ingredient ingredient : container.getIngredients()) {
            ingredient.displayInfo();
        }
        System.out.println();
    }
}
