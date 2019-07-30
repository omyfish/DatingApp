import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeAgoPipe} from 'time-ago-pipe';
import { PaginationModule } from 'ngx-bootstrap/pagination';

import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { appRoute } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { UserService } from './_services/user.service';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolver/member-detail.resolver';
import { MemberListResolver } from './_resolver/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolver/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { ListsResolver } from './_resolver/lists.resolver';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      ValueComponent,
      HomeComponent,
      NavComponent,
      RegisterComponent,
      MemberListComponent,
      MemberDetailComponent,
      MemberCardComponent,
      MemberEditComponent,
      PhotoEditorComponent,
      ListsComponent,
      MessagesComponent,
      TimeAgoPipe,
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      BsDatepickerModule.forRoot(),
      BrowserAnimationsModule,
      NgxGalleryModule,
      ButtonsModule.forRoot(),
      BsDropdownModule.forRoot(),
      TabsModule.forRoot(),
      FileUploadModule,
      PaginationModule.forRoot(),
      RouterModule.forRoot(appRoute),
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: ['192.168.0.3:5000'],
            blacklistedRoutes: ['192.168.0.3:5000/api/auth']
         }
      })
   ],
   providers: [
      AuthService,
      UserService,
      ErrorInterceptorProvider,
      AlertifyService,
      AuthGuard,
      PreventUnsavedChanges,
      MemberListResolver,
      MemberDetailResolver,
      MemberEditResolver,
      ListsResolver,
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
