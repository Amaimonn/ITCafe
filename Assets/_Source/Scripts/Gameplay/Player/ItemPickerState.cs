using System;
using System.Collections.Generic;
using ITCafe.CafeBusiness;
using ITCafe.Environment;
using R3;
using UnityEngine;

namespace ITCafe.Player
{
    // Базовое состояние сборщика предметов
    public abstract class ItemPickerState
    {
        protected readonly IItemPicker _picker;

        protected ItemPickerState(IItemPicker picker)
        {
            _picker = picker;
        }

        public abstract bool CanTake(IItem item);
        public abstract void Take(IItem item);

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }

    public class BusyHandsState : ItemPickerState
    {
        public BusyHandsState(IItemPicker picker) : base(picker)
        {
        }

        public override bool CanTake(IItem item)
        {
            return false;
        }

        public override void Take(IItem item)
        {
           
        }
    }

    // Состояние с пустыми руками
    public class EmptyHandsState : ItemPickerState
    {
        public EmptyHandsState(IItemPicker picker) : base(picker)
        {
        }

        public override bool CanTake(IItem item)
        {
            return item != null;
        }

        public override void Take(IItem item)
        {
            Debug.Log($"Taking item {item.transform.name} with empty hands");
            item.transform.parent = _picker.HoldingPoint;
            item.transform.SetLocalPositionAndRotation(-item.CenterOffset, Quaternion.identity);
            _picker.SetCurrentItem(item);

            if (item is IItemsContainer container)
                _picker.ChangeState(new WithContainerState(_picker, container));
            else
                _picker.ChangeState(new BusyHandsState(_picker));
            // else if (item is Plate)
            //     _picker.ChangeState(new WithPlateState(_picker, (Plate)item));
        }
    }

    public class WithContainerState : ItemPickerState
    {
        private readonly IItemsContainer _container;

        public WithContainerState(IItemPicker picker, IItemsContainer container) : base(picker)
        {
            _container = container;
        }

        public override bool CanTake(IItem item)
        {
            return item is IMenuItem menuItem && _container.CanTake(menuItem);
        }

        public override void Take(IItem item)
        {
            if (!CanTake(item) || item is not IMenuItem menuItem)
                return;

            Debug.Log($"Placing item {item.transform.name} on tray");
            _container.Take(menuItem);
        }
    }

    // // Состояние с тарелкой
    // public class WithPlateState : ItemPickerState
    // {
    //     private readonly Plate _plate;
    //     
    //     public WithPlateState(ItemPicker picker, Plate plate) : base(picker)
    //     {
    //         _plate = plate;
    //     }
    //     
    //     public override bool CanTake(IItem item)
    //     {
    //         // С тарелкой можем брать только определенные типы предметов
    //         // Например, ингредиенты для составления блюда
    //         return item is IIngredient ingredient && _plate.CanAddIngredient(ingredient);
    //     }
    //     
    //     public override void Take(IItem item)
    //     {
    //         if (!CanTake(item) || !(item is IIngredient ingredient)) return;
    //         
    //         Debug.Log($"Adding ingredient {item.transform.name} to plate");
    //         _plate.AddIngredient(ingredient);
    //     }
    //     
    //     public override bool TryTake(IItem item)
    //     {
    //         if (!CanTake(item)) return false;
    //         Take(item);
    //         return true;
    //     }
    //     
    //     public override void Drop()
    //     {
    //         Debug.Log("Dropping plate");
    //         _plate.transform.parent = null;
    //         _plate.transform.position = _picker.DropPoint.position;
    //         _plate.Drop();
    //         _picker.ChangeState(new EmptyHandsState(_picker));
    //     }
    //     
    //     public override void OnExit()
    //     {
    //         _plate.transform.parent = null;
    //         _plate.transform.position = _picker.DropPoint.position;
    //     }
    // }
}