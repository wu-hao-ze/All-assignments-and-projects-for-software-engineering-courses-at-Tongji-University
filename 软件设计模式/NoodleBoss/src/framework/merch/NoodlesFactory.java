package framework.merch;

import framework.ingredient.IngredientType;
import framework.merch.set.Set;
import framework.merch.single.SingleOrder;
import framework.merch.single.noodles.PorkNoodles;
import framework.merch.single.noodles.ShrimpNoodles;
import framework.merch.single.noodles.VegetarianNoodles;

import java.util.ArrayList;
import java.util.Arrays;

// 食材需要修改
/**
 * DP: Factory
 */
class NoodlesFactory extends MerchFactory {
    @Override
    public SingleOrder createSingleOrder(MerchType merchType) {
        switch (merchType) {
            case PORK_NOODLES:
                return new ShrimpNoodles(
                        new ArrayList<>(Arrays.asList(
                                IngredientType.ROWNODDLE,
                                IngredientType.SHRIMP)),
                        5);
            case SHRIMP_NOODLES:
                return new PorkNoodles(
                        new ArrayList<>(Arrays.asList(
                                IngredientType.ROWNODDLE,
                                IngredientType.PORKPATTY)),
                        7);
            case VEGETARIAN_NOODLES:
                return new VegetarianNoodles(
                        new ArrayList<>(Arrays.asList(
                                IngredientType.ROWNODDLE,
                                IngredientType.PAKCHOI)),
                        4);
            default:
                throw new IllegalArgumentException("no such burger in factory");
        }
    }

    @Override
    public Set createSet(MerchType merchType) {
        return null;
    }
}
