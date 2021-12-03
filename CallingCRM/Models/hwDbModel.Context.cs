﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CallingCRM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class hwLiveEntities : DbContext
    {
        public hwLiveEntities()
            : base("name=hwLiveEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<LeadFollowup> LeadFollowups { get; set; }
        public virtual DbSet<Lead> Leads { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<LeadForwarded> LeadForwardeds { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<LedgerHistory> LedgerHistories { get; set; }
        public virtual DbSet<InvestmentReturn> InvestmentReturns { get; set; }
        public virtual DbSet<Investor> Investors { get; set; }
        public virtual DbSet<Investment> Investments { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<SalaryRevision> SalaryRevisions { get; set; }
        public virtual DbSet<Payroll> Payrolls { get; set; }
        public virtual DbSet<ProductsService> ProductsServices { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
    
        public virtual ObjectResult<getBalancedNode_Result> getBalancedNode(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getBalancedNode_Result>("getBalancedNode", userIdParameter);
        }
    
        public virtual ObjectResult<GetExtreemLeg_Result> GetExtreemLeg(string parentNode, string legSide)
        {
            var parentNodeParameter = parentNode != null ?
                new ObjectParameter("ParentNode", parentNode) :
                new ObjectParameter("ParentNode", typeof(string));
    
            var legSideParameter = legSide != null ?
                new ObjectParameter("LegSide", legSide) :
                new ObjectParameter("LegSide", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetExtreemLeg_Result>("GetExtreemLeg", parentNodeParameter, legSideParameter);
        }
    
        public virtual ObjectResult<VeriFamilyMember_Result> VeriFamilyMember(string sponsorId, string parentId)
        {
            var sponsorIdParameter = sponsorId != null ?
                new ObjectParameter("SponsorId", sponsorId) :
                new ObjectParameter("SponsorId", typeof(string));
    
            var parentIdParameter = parentId != null ?
                new ObjectParameter("ParentId", parentId) :
                new ObjectParameter("ParentId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<VeriFamilyMember_Result>("VeriFamilyMember", sponsorIdParameter, parentIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> DeleteUser(string username)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("Username", username) :
                new ObjectParameter("Username", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("DeleteUser", usernameParameter);
        }
    
        public virtual ObjectResult<UpdateTeamCount_Result> UpdateTeamCount(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<UpdateTeamCount_Result>("UpdateTeamCount", userIdParameter);
        }
    
        public virtual ObjectResult<PendingRequestOfDOMCharges_Result> PendingRequestOfDOMCharges()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PendingRequestOfDOMCharges_Result>("PendingRequestOfDOMCharges");
        }
    
        public virtual ObjectResult<string> ProcessRequestOfDOMCharges(string requestId)
        {
            var requestIdParameter = requestId != null ?
                new ObjectParameter("RequestId", requestId) :
                new ObjectParameter("RequestId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ProcessRequestOfDOMCharges", requestIdParameter);
        }
    
        public virtual ObjectResult<DomTransactionWithName> GetDOMTransactions(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DomTransactionWithName>("GetDOMTransactions", userIdParameter);
        }
    
        public virtual ObjectResult<DomTransactionWithName> GetAllTransactions(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DomTransactionWithName>("GetAllTransactions", userIdParameter);
        }
    
        public virtual ObjectResult<DomTransactionWithName> GetWalletTransactions(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<DomTransactionWithName>("GetWalletTransactions", userIdParameter);
        }
    
        public virtual ObjectResult<GetTotalSummary_Result> GetTotalSummary(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTotalSummary_Result>("GetTotalSummary", userIdParameter);
        }
    
        public virtual ObjectResult<AddHelpPayStatus_Result> AddHelpPayStatus(string requestId)
        {
            var requestIdParameter = requestId != null ?
                new ObjectParameter("RequestId", requestId) :
                new ObjectParameter("RequestId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<AddHelpPayStatus_Result>("AddHelpPayStatus", requestIdParameter);
        }
    
        public virtual ObjectResult<getWaitingHelp_Result> getWaitingHelp()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getWaitingHelp_Result>("getWaitingHelp");
        }
    
        public virtual ObjectResult<getCommunityMembers_Result> getCommunityMembers(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getCommunityMembers_Result>("getCommunityMembers", userIdParameter);
        }
    
        public virtual ObjectResult<getCommunityMembersAdmin_Result> getCommunityMembersAdmin()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getCommunityMembersAdmin_Result>("getCommunityMembersAdmin");
        }
    
        public virtual ObjectResult<GetPayoutByUserId_Result> GetPayoutByUserId(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPayoutByUserId_Result>("GetPayoutByUserId", userIdParameter);
        }
    
        public virtual ObjectResult<getTransactions_Result> getTransactions(string userId, string purpose)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var purposeParameter = purpose != null ?
                new ObjectParameter("Purpose", purpose) :
                new ObjectParameter("Purpose", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getTransactions_Result>("getTransactions", userIdParameter, purposeParameter);
        }
    
        public virtual int ReduceTeamCount(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReduceTeamCount", userIdParameter);
        }
    
        public virtual ObjectResult<getNetworkResource_Result> getNetworkResource(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getNetworkResource_Result>("getNetworkResource", userIdParameter);
        }
    
        public virtual ObjectResult<UpdateTeamHelp_Result> UpdateTeamHelp(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<UpdateTeamHelp_Result>("UpdateTeamHelp", userIdParameter);
        }
    
        public virtual int ReverseTransaction(Nullable<int> txnId)
        {
            var txnIdParameter = txnId.HasValue ?
                new ObjectParameter("TxnId", txnId) :
                new ObjectParameter("TxnId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReverseTransaction", txnIdParameter);
        }
    
        public virtual ObjectResult<UpdateGrowthRate_Result> UpdateGrowthRate(string username, string requestId)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("Username", username) :
                new ObjectParameter("Username", typeof(string));
    
            var requestIdParameter = requestId != null ?
                new ObjectParameter("RequestId", requestId) :
                new ObjectParameter("RequestId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<UpdateGrowthRate_Result>("UpdateGrowthRate", usernameParameter, requestIdParameter);
        }
    
        public virtual ObjectResult<getBinaryMatch_Result> getBinaryMatch(string userId, Nullable<System.DateTime> date)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getBinaryMatch_Result>("getBinaryMatch", userIdParameter, dateParameter);
        }
    
        public virtual ObjectResult<BinaryBringForward_Result> BinaryBringForward(string userId, Nullable<System.DateTime> payDate, string payMode)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var payDateParameter = payDate.HasValue ?
                new ObjectParameter("payDate", payDate) :
                new ObjectParameter("payDate", typeof(System.DateTime));
    
            var payModeParameter = payMode != null ?
                new ObjectParameter("payMode", payMode) :
                new ObjectParameter("payMode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<BinaryBringForward_Result>("BinaryBringForward", userIdParameter, payDateParameter, payModeParameter);
        }
    
        public virtual ObjectResult<WalletSummary_Result> walletSummary(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WalletSummary_Result>("walletSummary", userIdParameter);
        }
    
        public virtual ObjectResult<CheckBinaryQualification_Result> CheckBinaryQualification(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CheckBinaryQualification_Result>("CheckBinaryQualification", userIdParameter);
        }
    
        public virtual int sendValueToUpline(string requestId)
        {
            var requestIdParameter = requestId != null ?
                new ObjectParameter("RequestId", requestId) :
                new ObjectParameter("RequestId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sendValueToUpline", requestIdParameter);
        }
    
        public virtual ObjectResult<getPayDetails_Result> getPayDetails(Nullable<int> payId)
        {
            var payIdParameter = payId.HasValue ?
                new ObjectParameter("payId", payId) :
                new ObjectParameter("payId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getPayDetails_Result>("getPayDetails", payIdParameter);
        }
    
        public virtual ObjectResult<GetTeamHelpAmount_Result> GetTeamHelpAmount(string userId, string treePosition, Nullable<System.DateTime> payDate)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var treePositionParameter = treePosition != null ?
                new ObjectParameter("TreePosition", treePosition) :
                new ObjectParameter("TreePosition", typeof(string));
    
            var payDateParameter = payDate.HasValue ?
                new ObjectParameter("payDate", payDate) :
                new ObjectParameter("payDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTeamHelpAmount_Result>("GetTeamHelpAmount", userIdParameter, treePositionParameter, payDateParameter);
        }
    
        public virtual ObjectResult<GetReferralHelpAmount_Result> GetReferralHelpAmount(string userId, Nullable<System.DateTime> payDate)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var payDateParameter = payDate.HasValue ?
                new ObjectParameter("payDate", payDate) :
                new ObjectParameter("payDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetReferralHelpAmount_Result>("GetReferralHelpAmount", userIdParameter, payDateParameter);
        }
    
        public virtual ObjectResult<GetUpcomingReferralHelpAmount_Result> GetUpcomingReferralHelpAmount(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUpcomingReferralHelpAmount_Result>("GetUpcomingReferralHelpAmount", userIdParameter);
        }
    
        public virtual ObjectResult<GetUpcomingTeamHelpAmount_Result> GetUpcomingTeamHelpAmount(string userId, string treePosition)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var treePositionParameter = treePosition != null ?
                new ObjectParameter("TreePosition", treePosition) :
                new ObjectParameter("TreePosition", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUpcomingTeamHelpAmount_Result>("GetUpcomingTeamHelpAmount", userIdParameter, treePositionParameter);
        }
    
        public virtual ObjectResult<getPaidReferrals_Result> getPaidReferrals(string userID)
        {
            var userIDParameter = userID != null ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getPaidReferrals_Result>("getPaidReferrals", userIDParameter);
        }
    
        public virtual ObjectResult<pullAmountFromWallet_Result> pullAmountFromWallet(string fromUserName, string toUserName, Nullable<int> txnAmount)
        {
            var fromUserNameParameter = fromUserName != null ?
                new ObjectParameter("FromUserName", fromUserName) :
                new ObjectParameter("FromUserName", typeof(string));
    
            var toUserNameParameter = toUserName != null ?
                new ObjectParameter("ToUserName", toUserName) :
                new ObjectParameter("ToUserName", typeof(string));
    
            var txnAmountParameter = txnAmount.HasValue ?
                new ObjectParameter("TxnAmount", txnAmount) :
                new ObjectParameter("TxnAmount", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<pullAmountFromWallet_Result>("pullAmountFromWallet", fromUserNameParameter, toUserNameParameter, txnAmountParameter);
        }
    
        public virtual int ScheduleGrowth(string requestId)
        {
            var requestIdParameter = requestId != null ?
                new ObjectParameter("RequestId", requestId) :
                new ObjectParameter("RequestId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ScheduleGrowth", requestIdParameter);
        }
    
        public virtual ObjectResult<sendAmountToWallet_Result> sendAmountToWallet(string fromUserName, string toUserName, Nullable<int> txnAmount)
        {
            var fromUserNameParameter = fromUserName != null ?
                new ObjectParameter("FromUserName", fromUserName) :
                new ObjectParameter("FromUserName", typeof(string));
    
            var toUserNameParameter = toUserName != null ?
                new ObjectParameter("ToUserName", toUserName) :
                new ObjectParameter("ToUserName", typeof(string));
    
            var txnAmountParameter = txnAmount.HasValue ?
                new ObjectParameter("TxnAmount", txnAmount) :
                new ObjectParameter("TxnAmount", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sendAmountToWallet_Result>("sendAmountToWallet", fromUserNameParameter, toUserNameParameter, txnAmountParameter);
        }
    }
}
