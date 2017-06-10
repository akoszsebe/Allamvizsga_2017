
using Allamvizsga2017.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Allamvizsga2017
{
    [Activity(Label = "DeviceSettingActivity", WindowSoftInputMode = SoftInput.StateHidden  , 
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DeviceSettingActivity : AppCompatActivity
    {
        string houseid { get; set; } = "";
        int devicevalue { get; set; } = 0;
        int original_value { get; set; } = 0;
        int value_delay { get; set; } = 0;
        int saveicon_id { get; set; } = 0;
        EditText etdevicename { get; set; }
        RecyclerView mRecyclerView { get; set; }
        RecyclerView.LayoutManager mLayoutManager { get; set; }
        MyIconListAdapter mAdapter { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DeviceSetting);

            string devicename = Intent.GetStringExtra("device_name");
            devicevalue = Intent.GetIntExtra("device_value", 0);
            value_delay = Intent.GetIntExtra("value_delay", 0);
            houseid = Intent.GetStringExtra("house_id");
            original_value = Intent.GetIntExtra("original_value", 0);
            int iconid = Intent.GetIntExtra("icon_id", 0);

            var tbdevicevalue = FindViewById<TextView>(Resource.Id.textViewDeviceWatt);
            var ivicon = FindViewById<ImageView>(Resource.Id.imageView1);
            etdevicename = FindViewById<EditText>(Resource.Id.textInputEditText1);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            var numberpicker = FindViewById<NumberPicker>(Resource.Id.numberPickerValueDelay);
            var btediticon = FindViewById<Button>(Resource.Id.buttonediticon);
            var layouticons = FindViewById<LinearLayout>(Resource.Id.linearLayoutIcons);
            var scrollview = FindViewById<ScrollView>(Resource.Id.scrollView1);

            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Device Settings";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            ivicon.SetImageResource(iconid);
            etdevicename.Hint = devicename;
            etdevicename.Text = devicename;

            numberpicker.MinValue = 0;
            numberpicker.MaxValue = 1000;
            numberpicker.Value = value_delay;


            etdevicename.Selected = false;
            etdevicename.Click += delegate { etdevicename.SetCursorVisible(true); };
            etdevicename.KeyPress += (s, e) =>
            {
                if (e.KeyCode == Keycode.Enter)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(etdevicename.WindowToken, 0);
                    etdevicename.SetCursorVisible(false);
                }
                else
                {
                    e.Handled = false;
                }
            };

            tbdevicevalue.Text = devicevalue.ToString();

            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            IconList icons = new IconList();
            mAdapter = new MyIconListAdapter(icons);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            saveicon_id = iconid;

            mAdapter.ItemClick += (sender, position) =>
            {
                ivicon.SetImageResource(icons.Iconids[position]);
                saveicon_id = icons.Iconids[position];
            };

            numberpicker.ValueChanged += delegate
            {
                 value_delay = numberpicker.Value;
            };

            var buttonin = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.abc_fade_in);
            btediticon.Click += delegate
            {
                btediticon.StartAnimation(buttonin);
                layouticons.LayoutParameters.Height = (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, 60, Resources.DisplayMetrics);
                mRecyclerView.SetAdapter(mAdapter);
            };

            scrollview.Clickable = true;
            scrollview.Click += (e,s) => 
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
                etdevicename.SetCursorVisible(false);
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_devicesetting, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                NotificationStarter.SetNotification_Enabled(false);
                Finish();
            }
            if (item.ItemId == Resource.Id.menu_save)
            {
                if (!etdevicename.Text.ToLower().Contains("unknown"))
                    if (saveicon_id != Resource.Drawable.unknownicon)
                    {
                        if (etdevicename.Text == "")
                        {
                            if (!etdevicename.Hint.ToLower().Contains("unknown"))
                                RestClient.SetDeviceSetting(houseid, etdevicename.Hint, saveicon_id, original_value, value_delay);
                        }
                        else
                            RestClient.SetDeviceSetting(houseid, etdevicename.Text, saveicon_id, original_value, value_delay);
                    }
                Finish();
            }
            if (item.ItemId == Resource.Id.menu_delete)
            {
                RestClient.DeleteDeviceSetting(houseid, original_value);
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
            etdevicename.SetCursorVisible(false);
            return base.OnTouchEvent(e);
        }
    }
    public class IconList
    {
        public List<int> Iconids { get; } = new List<int>();
        public IconList()
        {
            Iconids.Add(Resource.Drawable.Blender);
            Iconids.Add(Resource.Drawable.Coffee_machine);
            Iconids.Add(Resource.Drawable.Air_conditioner);
            Iconids.Add(Resource.Drawable.Audio_speakers);
            Iconids.Add(Resource.Drawable.Stove);
            Iconids.Add(Resource.Drawable.Toaster);
            Iconids.Add(Resource.Drawable.Curling_iron);
            Iconids.Add(Resource.Drawable.Laptop);
            Iconids.Add(Resource.Drawable.Dishwashing_machine);
            Iconids.Add(Resource.Drawable.Drilling_machine);
            Iconids.Add(Resource.Drawable.Fan);
            Iconids.Add(Resource.Drawable.Flex);
            Iconids.Add(Resource.Drawable.Hailr_straightener);
            Iconids.Add(Resource.Drawable.Hair_dryer);
            Iconids.Add(Resource.Drawable.Hand_mixer);
            Iconids.Add(Resource.Drawable.Lightning_bulb);
            Iconids.Add(Resource.Drawable.Microwave);
            Iconids.Add(Resource.Drawable.Radio);
            Iconids.Add(Resource.Drawable.Refrigerator);
            Iconids.Add(Resource.Drawable.Sandwich_maker);
            Iconids.Add(Resource.Drawable.Smoke_extractor);
            Iconids.Add(Resource.Drawable.Television);
            Iconids.Add(Resource.Drawable.Teamashine);
            Iconids.Add(Resource.Drawable.Stand_mixer);
            Iconids.Add(Resource.Drawable.Vacuum_cleaner);
            Iconids.Add(Resource.Drawable.Water_heater);
        }

    }

    public class IconViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }

        public IconViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageViewForIcon);
            itemView.Click += (sender, e) => listener(Position);
        }
    }


    public class MyIconListAdapter : RecyclerView.Adapter
    {
        private IconList icons;

        public event EventHandler<int> ItemClick;

        public MyIconListAdapter(IconList icons)
        {
            this.icons = icons;
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.iconCardView_item, parent, false);

            IconViewHolder vh = new IconViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            IconViewHolder vh = holder as IconViewHolder;
            vh.Image.SetImageResource(icons.Iconids[position]);
        }

        public override int ItemCount
        {
            get { return icons.Iconids.Count; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}