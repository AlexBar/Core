#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Tests
{
    using Moq;
    using Xunit;

    public class OrderableTaskTests
    {
        [Fact]
        public void Order_should_be_set_to_default_order()
        {
            Assert.Equal(OrderableTask.DefaultOrder, new Mock<OrderableTask>().Object.Order);
        }
    }
}