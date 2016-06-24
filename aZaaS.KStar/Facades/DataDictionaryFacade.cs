using aZaaS.KStar.DataDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.Framework.Extensions;
using AutoMapper;
using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar.Facades
{
    public class DataDictionaryFacade
    {
        private DataDictionaryManager _dataDictionaryManager;

        public DataDictionaryFacade()
        {
            _dataDictionaryManager = new DataDictionaryManager();
        }

        public IEnumerable<DataDictionaryWithChildDto> GetAllDataDicCategory()
        {
            return Mapper.Map<IEnumerable<DataDictionaryEntity>, IEnumerable<DataDictionaryWithChildDto>>(_dataDictionaryManager.GetAllDataDicCategory());
        }

        public IEnumerable<DataDictionaryWithChildDto> GetDataDicCategoryByParentId(Guid parentid)
        {
            parentid.NullThrowArgumentEx("parentid");

            return Mapper.Map<IEnumerable<DataDictionaryEntity>, IEnumerable<DataDictionaryWithChildDto>>(_dataDictionaryManager.GetDataDicCategoryByParentId(parentid));
        }

        public IEnumerable<DataDictionaryWithChildDto> GetDataDicCategoryByParentId(string code)
        {
            code.NullOrEmptyThrowArgumentEx("code");

            return Mapper.Map<IEnumerable<DataDictionaryEntity>, IEnumerable<DataDictionaryWithChildDto>>(_dataDictionaryManager.GetDataDicCategoryByCode(code));
        }

        public IEnumerable<DataDictionaryWithChildDto> GetDataDictionaryByParentId(Guid parentid)
        {
            parentid.NullThrowArgumentEx("parentid");

            return Mapper.Map<IEnumerable<DataDictionaryEntity>, IEnumerable<DataDictionaryWithChildDto>>(_dataDictionaryManager.GetDataDictionaryByParentId(parentid));
        }

        public IEnumerable<DataDictionaryWithChildDto> GetDataDictionaryByCode(string code)
        {
            code.NullOrEmptyThrowArgumentEx("code");

            return Mapper.Map<IEnumerable<DataDictionaryEntity>, IEnumerable<DataDictionaryWithChildDto>>(_dataDictionaryManager.GetDataDictionaryByCode(code));
        }

        public DataDictionaryWithChildDto GetDataDictionaryById(Guid id)
        {
            id.NullThrowArgumentEx("parentid");

            return Mapper.Map<DataDictionaryEntity, DataDictionaryWithChildDto>(_dataDictionaryManager.GetDataDictionaryById(id));
        }

        public Guid AddDataDictionary(DataDictionaryBaseDto datadic)
        {
            datadic.NullThrowArgumentEx("DataDictionary");

            return _dataDictionaryManager.AddDataDictionary(datadic);
        }

        public void EditDataDictionary(DataDictionaryBaseDto datadic)
        {
            datadic.NullThrowArgumentEx("DataDictionary");

            _dataDictionaryManager.EditDataDictionary(datadic);
        }

        public void DelDataDictionary(Guid id)
        {
            id.NullThrowArgumentEx("id");

            _dataDictionaryManager.DelDataDictionary(id);
        }

        public string GetDataDicValue(string key)
        {
            key.NullThrowArgumentEx("key");

            return _dataDictionaryManager.GetDataDicValue(key);
        }

        public void DelDataDicByParentId(Guid parentId)
        {
            parentId.NullThrowArgumentEx("parentId");

            _dataDictionaryManager.DelDataDicByParentId(parentId);
        }

        public bool ExistCode(string code, bool iscategory)
        {
            code.NullOrEmptyThrowArgumentEx("code");
            iscategory.NullThrowArgumentEx("iscategory");
            return _dataDictionaryManager.ExistCode(code, iscategory);
        }
    }
}
