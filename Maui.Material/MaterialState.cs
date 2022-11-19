namespace Maui.Material;

public enum MaterialState
{
    /// The state when the user drags their mouse cursor over the given widget.
    ///
    /// See: https://material.io/design/interaction/states.html#hover.
    Hovered,

    /// The state when the user navigates with the keyboard to a given widget.
    ///
    /// This can also sometimes be triggered when a widget is tapped. For example,
    /// when a [TextField] is tapped, it becomes [focused].
    ///
    /// See: https://material.io/design/interaction/states.html#focus.
    Focused,

    /// The state when the user is actively pressing down on the given widget.
    ///
    /// See: https://material.io/design/interaction/states.html#pressed.
    Pressed,

    /// The state when this widget is being dragged from one place to another by
    /// the user.
    ///
    /// https://material.io/design/interaction/states.html#dragged.
    Dragged,

    /// The state when this item has been selected.
    ///
    /// This applies to things that can be toggled (such as chips and checkboxes)
    /// and things that are selected from a set of options (such as tabs and radio buttons).
    ///
    /// See: https://material.io/design/interaction/states.html#selected.
    Selected,

    /// The state when this widget overlaps the content of a scrollable below.
    ///
    /// Used by [AppBar] to indicate that the primary scrollable's
    /// content has scrolled up and behind the app bar.
    ScrolledUnder,

    /// The state when this widget is disabled and cannot be interacted with.
    ///
    /// Disabled widgets should not respond to hover, focus, press, or drag
    /// interactions.
    ///
    /// See: https://material.io/design/interaction/states.html#disabled.
    Disabled,

    /// The state when the widget has entered some form of invalid state.
    ///
    /// See https://material.io/design/interaction/states.html#usage.
    Error,
}
