# Defining custom USS styles

The first step to define a USS style is to create a new stylesheet. Stylesheets can be used to extend the Editor’s visual appearance.
This can be done by adding a file named `common.uss` in an Editor folder in a `StyleSheets/Extensions` folder hierarchy.
For example, the following locations are valid:

``` txt
Assets/Editor/Stylesheets/Extensions/common.uss
Assets/Editor/MyFolder/Stylesheets/Extensions/common.uss
Assets/Editor/MyFolder1/MyFolder2/Stylesheets/Extensions/common.uss
```

USS files (for Unity Style Sheet) use a [CSS-like syntax to describe new styles](https://docs.unity3d.com/Manual/UIE-USS.html). Here is an example:

``` css
myStyle
{
    width:18px;
    height:18px;
    background-image: resource("Assets/Editor/icon.png");
}
```

In this style, we specified that we wish to use a custom icon along with size properties. USS styles also support [pseudo-states](https://docs.unity3d.com/Manual/UIE-USS-Selectors.html), which works like [pseudo-classes in CSS](https://developer.mozilla.org/en-US/docs/Web/CSS/Pseudo-classes). Timeline markers support the following pseudo-states:

- Collapsed: `.myStyle`
- Normal: `.myStyle:checked`
- Selected: `.myStyle:hover:focus:checked`

USS stylesheets also support Unity's light and dark themes. Styles in files named `dark.uss` and `light.uss` will be used as an override of the style in `common.uss`. For example:

- `common.uss`

``` css
myStyle
{
   width:18px;
   height:18px;
   color: rgb(125, 125, 125);
}
```

- `dark.uss`

``` css
myStyle
{
   color: rgb(0, 0, 0);
   background-image: resource("icon_dark.png");
}
```

- `light.uss`

``` css
myStyle
{
   color: rgb(255, 255, 255);
   background-image: resource("icon_light.png");
}
```

In the dark theme, `myStyle` will be resolved to:

``` css
myStyle
{
   width:18px;
   height:18px;
   color: rgb(0, 0, 0);
   background-image: resource("icon_dark.png");
}
```

and in the light theme:

``` css
myStyle
{
   width:18px;
   height:18px;
   color: rgb(255, 255, 255);
   background-image: resource("icon_light.png");
}
```
