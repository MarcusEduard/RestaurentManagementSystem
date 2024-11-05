﻿using DotNet8.PosFrontendBlazor.Models.Customer;
using DotNet8.PosFrontendBlazor.Models.State;

namespace DotNet8.PosFrontendBlazor.Pages.Customer;

public partial class P_Customer
{
    private CustomerListResponseModel? ResponseModel;

    private int _pageNo = 1;
    private int _pageSize = 10;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InjectService.EnableLoading();
            await List();
            StateHasChanged();
            await InjectService.DisableLoading();
        }
    }

    private async Task List()
    {
        ResponseModel = await HttpClientService.ExecuteAsync<CustomerListResponseModel>(
            $"{Endpoints.Customer}/{_pageNo}/{_pageSize}",
            EnumHttpMethod.Get
        );
    }

    private async Task Popup()
    {
        DialogResult result = await InjectService.ShowModalBoxAsync<P_CustomerDialog>("New Customer");

        if (!result.Canceled)
            await List();
    }

    private async Task EditPopup(int CustomerId, string CustomerName, string MobileNo, DateTime? DateOfBirth, string Gender, string StateCode, string TownshipCode)
    {
        StateListResponseModel stateListResponseModel = await HttpClientService.ExecuteAsync<StateListResponseModel>(
            $"{Endpoints.State}",
            EnumHttpMethod.Get);

        TownshipListResponseModel townshipListResponseModel = await HttpClientService.ExecuteAsync<TownshipListResponseModel>(
            $"{Endpoints.Township}/GetTownshipByStateCode/{StateCode}",
            EnumHttpMethod.Get);

        CustomerParamsModel model = new(CustomerId, CustomerName, MobileNo, DateOfBirth, Gender, StateCode, TownshipCode);
       
        var parameters = new DialogParameters<P_CustomerDialog>
        {
            { x => x.model, model },
            {x => x.stateListResponseModel, stateListResponseModel },
            {x => x.townshipListResponseModel, townshipListResponseModel }
        };

        DialogResult result = await InjectService.ShowModalBoxAsync<P_CustomerDialog>("Edit Customer", parameters);

        if (!result.Canceled)
            await List();
    }

    private async Task DeletePopUp(int id)
    {
        var parameters = new DialogParameters<P_DeleteCustomerDialog>()
        {
            {"id", id}
        };
        DialogResult result = await InjectService.ShowModalBoxAsync<P_DeleteCustomerDialog>("Delete Customer", parameters);

        if (!result.Canceled)
            await List();
    }

    private async Task PageChanged(int i)
    {
        _pageNo = i;
        await List();
    }
}

public class StateResponseModel
{
    public StateModel Data { get; set; }
}