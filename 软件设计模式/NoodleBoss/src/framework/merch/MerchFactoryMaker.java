package framework.merch;

/**
 * Abstract Factory, Null Object
 */
public class MerchFactoryMaker {
    public enum MerchFactoryType {
        BEVERAGE,
        NOODLES,
        SET,
        NULL
    }

    public static MerchFactory create(MerchFactoryType type) {
        switch (type) {
            case SET:
                return new SetFactory();
            case NOODLES:
                return new NoodlesFactory();
            case BEVERAGE:
                return new BeverageFactory();
            default:
                return new NullMerchFactory();
        }
    }
}
