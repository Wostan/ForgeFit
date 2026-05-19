using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Handlers.Items;
using Object = Java.Lang.Object;

namespace ForgeFit.MAUI.Handlers;

public class NoSwipeCarouselViewHandler : CarouselViewHandler
{
#if ANDROID
    protected override void ConnectHandler(RecyclerView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.SetScrollingTouchSlop(RecyclerView.TouchSlopPaging);
        platformView.AddOnItemTouchListener(new NoSwipeTouchListener());
    }

    private class NoSwipeTouchListener : Object, RecyclerView.IOnItemTouchListener
    {
        public bool OnInterceptTouchEvent(RecyclerView rv, MotionEvent e)
        {
            return e.Action == MotionEventActions.Move;
        }

        public void OnTouchEvent(RecyclerView rv, MotionEvent e)
        {
        }

        public void OnRequestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
        }
    }
#endif
}
