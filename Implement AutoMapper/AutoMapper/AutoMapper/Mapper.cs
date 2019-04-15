using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace AutoMapper
{
    public class Mapper
    {
        public TDest CreateMappedObject<TDest>(object source)
        {
            if(source == null)
            {
                throw new ArgumentException(ExceptionUtils.NullSource);
            }

            var dest = Activator.CreateInstance(typeof(TDest));
            return (TDest)this.MapObject(source, dest);
        }

        private object MapObject(object source, object dest)
        {
            foreach (var destProp in dest.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite))
            {
                var sourceProp = source.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.Name == destProp.Name);

                if (sourceProp != null)
                {
                    var sourceValue = sourceProp.GetMethod.Invoke(source, new object[0]);

                    if (sourceValue == null)
                    {
                        throw new ArgumentException(ExceptionUtils.NullableSourceValueGetMethod);
                    }

                    if (ReflectionUtils.IsPrimitive(sourceValue.GetType()))
                    {
                        destProp.SetValue(dest, sourceValue);
                        continue;
                    }

                    if (ReflectionUtils.IsGenericCollection(sourceValue.GetType()))
                    {

                        if (ReflectionUtils.IsPrimitive(sourceValue.GetType().GetGenericArguments()[0]))
                        {
                            var destinationCollection = sourceValue;
                            destProp.SetMethod.Invoke(dest, new[] { destinationCollection });
                        }
                        else
                        {
                            var destColl = destProp.GetMethod.Invoke(dest, new object[0]);
                            var destType = destColl.GetType().GetGenericArguments()[0];

                            foreach (var value in (IEnumerable)sourceValue)
                            {
                                ((IList)destColl).Add(this.CraetMappedObject(value, destType));
                            }
                        }
                    }
                    else if (ReflectionUtils.IsNonGenericCollection(sourceValue.GetType()))
                    {
                        int sourceCollectionLength = ((object[])sourceValue).Length;
                        var destColl = (IList)Activator.CreateInstance(destProp.PropertyType,
                            new object[] { sourceCollectionLength });

                        for (int i = 0; i < sourceCollectionLength; i++)
                        {
                            destColl[i] = this.CraetMappedObject(((object[])sourceValue)[i], destProp.PropertyType.GetElementType());
                        }

                        destProp.SetValue(dest, destColl);
                    }
                    else
                    {
                        var value = this.CraetMappedObject(sourceValue, destProp.PropertyType);
                        destProp.SetValue(dest, value);
                    }
                }
            }

            return dest;
        }

        private object CraetMappedObject(object source, Type destType)
        {
            if(source == null || destType == null)
            {
                throw new ArgumentException(ExceptionUtils.SourceOrDestTypeIsNull);
            }

            var dest = Activator.CreateInstance(destType);
            return MapObject(source, dest);
        }
    }
}
