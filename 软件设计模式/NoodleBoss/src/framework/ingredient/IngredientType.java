package framework.ingredient;

public enum IngredientType {
    SHRIMP("虾仁"), PORKPATTY("大排"), PAKCHOI("青菜"), ROWNODDLE("面条");

    String name;

    IngredientType(String name) {
        this.name = name;
    }

    @Override
    public String toString() {
        return name;
    }
}
