﻿using SourceCodeGenerator.CodeGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeGenerator.CodeTemplates
{
    public class CreateRequestValidator
    {
        private ModelInfo _modelInfo = new ModelInfo();
        private Type _model;
        private string _codefolder;
        private CodeGeneratorEngine _engine;
        private string _category;
        private string _properties;

        public CreateRequestValidator(CodeGeneratorEngine engine, bool BackupOriginal = true)
        {
            _engine = engine;
            _modelInfo = _engine.ModelInfo;
            _model = _engine.Model;
            _codefolder = @$"Core\Application\Validators\";
            string fileName = @$"Create{_model.Name}RequestValidator.cs";
            _category = string.IsNullOrEmpty(_modelInfo.Category) ? string.Empty : $@".{_modelInfo.Category}";
            _properties = _engine.GetValidationRules();

            string rootPath = EngineFunctions.GetApplicationPath();
            string codePath = @$"{rootPath}{_codefolder}" + (string.IsNullOrEmpty(_modelInfo.Category) ? @$"\" : @$"\{_modelInfo.Category}\");
            Directory.CreateDirectory(codePath);
            string filePath = codePath + fileName;
            string code = GenerateCode();
            if (BackupOriginal) EngineFunctions.BackupFile(filePath);
            File.WriteAllText(filePath, code);
        }

        private string GenerateCode()
        {
            string iCode = @$"// Autogenerated by SourceCodeGenerator

using System;
using {_engine.AppName}.Shared.DTOs{_category};
using FluentValidation;

namespace {_engine.AppName}.Application.Validators{_category}
{{
    public class Create{_model.Name}RequestValidator : CustomValidator<Create{_model.Name}Request>
    {{
        public Create{_model.Name}RequestValidator()
        {{
{_properties}
        }}
    }}
}}";
            return iCode;
        }
    }
}