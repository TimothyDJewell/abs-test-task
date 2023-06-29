using ClosedXML.Excel;

namespace ABS.FileGeneration.Test
{
    [TestFixture]
    public class WorkbookGeneratorTest
    {
        [Test]
        public void Create_OnlyPath_IssuesExpectedCalls()
        {
            var subWorkbook = Substitute.For<IXLWorkbook>();
            var subject = Substitute.ForPartsOf<WorkbookGenerator>(); // using a substitute so we can fake out CreateInMemoryWorkbook
            subject.CreateInMemoryWorkbook().Returns(subWorkbook);
            var fileName = new TemporaryFilePath(@"fake file path!");

            subject.Create(fileName);

            Assert.That(
                subWorkbook.ReceivedCalls().Select(c => c.GetMethodInfo().Name),
                Is.EquivalentTo(new[]
                {
                    nameof(IXLWorkbook.AddWorksheet),
                    nameof(IXLWorkbook.SaveAs),
                    nameof(IXLWorkbook.Dispose),
                }));
            subWorkbook.Received(1).AddWorksheet();
            subWorkbook.Received(1).SaveAs(@"fake file path!");
            subWorkbook.Received(1).Dispose();
        }

        [Test]
        public void CreateInMemoryWorkbook_NormalInstance_ReturnsCorrectInstance()
        {
            var subject = new WorkbookGenerator();

            IXLWorkbook result = subject.CreateInMemoryWorkbook();

            Assert.That(result, Is.InstanceOf<XLWorkbook>());
        }
    }
}
