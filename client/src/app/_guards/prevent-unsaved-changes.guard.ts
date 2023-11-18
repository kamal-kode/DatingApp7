import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';
// When user navigate without save it will display the alert message
export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component, currentRoute, currentState, nextState) => {
  if(component.editForm?.dirty) {
    const confirmService = inject(ConfirmService);
  return confirmService.confirm();
  }
  return true;
};
