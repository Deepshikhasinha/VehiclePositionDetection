<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VehiclePositionRecoknition_POC._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="row">
        <div class="col-md-12">
            <%--<input name="ImageURL" class="imageURL-input" onclick="$(this).val('');" id="ImageURLInput" type="text" value="Enter Image URL" />--%>
            <asp:TextBox ID="TextBox1" class="url-input" onclick="$(this).val('');" runat="server" value="Enter Image URL"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="GO" OnClick="Button1_Click" />              
        </div>
    </div>
    
    <div class="row">
        
        <div class="col-md-9">            
            <asp:Image id="ImageURL" runat="server" class="img-responsive"  />
        </div>
        <div class="col-md-3">
        
            <label class="results-label">Results</label>
            <label id="ResultsLabel"></label>
            <hr/>
            <asp:GridView ID="GridView1" GridLines="None" CssClass="gridview" runat="server"></asp:GridView>
            
            <hr/>            
        </div>
    </div>

</asp:Content>
