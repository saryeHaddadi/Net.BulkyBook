using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility;

public static class SD
{
	// Roles
	public const string ROLE_INDIVIDUAL = "Individual";
	public const string ROLE_COMPANY = "Company";
	public const string ROLE_ADMIN = "Admin";
	public const string ROLE_EMPLOYEE = "Employee";

	// Order status
	public const string StatusPending = "Pending";
	public const string StatusApproved = "Approved";
	public const string StatusInProcess = "InProcess";
	public const string StatusShipped = "Shipped";
	public const string StatusCancelled = "Cancelled";
	public const string StatusRefunded = "Refunded";

	// Payment Status
	public const string PaymentStatusPending = "Pending";
	public const string PaymentStatusApproved = "Approved";
	public const string PaymentStatusDelayedPayement = "ApprovedForDelayedPayement";
	public const string PaymentStatusRejected = "Rejected";

	// Other
	public const string SessionCartItemCount = "SessionCartItemCount";
}
