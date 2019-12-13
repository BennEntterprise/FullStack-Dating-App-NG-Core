import { Injectable } from "@angular/core";
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent>{
    canDeactivate(component: MemberEditComponent) {
        // Tests to ensure they want to navigate away
        if (component.editForm.dirty) {
            return confirm('Are you sure you want to continue? Any unsave changes will be lost...');
        }
        // Allows navigation if the from isn't dirty
        return true;
    }
}