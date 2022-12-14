using FluentValidation.Extensions.Tests.Models;
using FluentValidation.Extensions.Tests.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentValidation.Extensions.Tests
{
    [TestClass]
    public class MultivalidatorTests
    {
        private readonly MultiValidator _validator;

        private readonly ExampleModel _exampleModel = ExampleModel.Example();

        public MultivalidatorTests()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<ExampleModelValidator>();
            var provider = sc.BuildServiceProvider();
            var factory = new ValidatorFactory(provider);

            _validator = new MultiValidator(factory);
        }

        [TestMethod]
        public async Task ValidateAsync_WithPassingValidator_Succeeds()
        {
            await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Validate_WithPassingValidator_Succeeds()
        {
            _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                      .Validate();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task ValidateAsync_WithFailingValidator_Throws()
        {
            _exampleModel.ExampleString = null;

            await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                            .ValidateAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validate_WithFailingValidator_Throws()
        {
            _exampleModel.ExampleString = null;

            _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                      .Validate();
        }

        [TestMethod]
        public async Task ValidateAsync_WithMultipleFailingValidators_ConcatenatesErrors()
        {
            var model2 = ExampleModel.Example();

            _exampleModel.ExampleString = null;
            model2.ExampleString = null;

            try
            {
                await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                                .Check(model2).Using<ExampleModelValidator>()
                                .ValidateAsync();

                Assert.IsTrue(false);
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual(2, ex.Errors.Count());
            }
        }

        [TestMethod]
        public void Validate_WithMultipleFailingValidators_ConcatenatesErrors()
        {
            var model2 = ExampleModel.Example();

            _exampleModel.ExampleString = null;
            model2.ExampleString = null;

            try
            {
                _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                          .Check(model2).Using<ExampleModelValidator>()
                          .Validate();

                Assert.IsTrue(false);
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual(2, ex.Errors.Count());
            }
        }

        [TestMethod]
        public async Task MultiValidator_WithOptionalNullObjectToValidate_Succeeds()
        {
            ExampleModel? nullModel = default(ExampleModel);
            await _validator.CheckOptional(nullModel).Using<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ValidationBuilder_WithOptionalNullObjectToValidate_Succeeds()
        {
            ExampleModel? nullModel = default(ExampleModel);
            await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                            .CheckOptional(nullModel).Using<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }
    }
}