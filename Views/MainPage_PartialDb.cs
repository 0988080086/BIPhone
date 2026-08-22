using System;
using Microsoft.Maui.Controls;

namespace BIPhone.Views;

public partial class MainPage : ContentPage
{
    private void LoadDashboardData()
    {
        if (lblDbRevenue != null) lblDbRevenue.Text = "125.000.000 đ";
        if (lblDbOrders != null) lblDbOrders.Text = "48";
        if (lblDbCalls != null) lblDbCalls.Text = "156";
        if (lblDbCustomers != null) lblDbCustomers.Text = "12";
    }
}